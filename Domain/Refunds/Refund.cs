using Microsoft.EntityFrameworkCore.Query.Internal;
using PaymentAPI.Domain;
using PaymentAPI.Domain.Payments;
using PaymentAPI.Domain.Primitives;
using PaymentAPI.DTO.refund;
using PaymentAPI.Primitives;
using System.Net.NetworkInformation;

namespace PaymentAPI.Domain.Refunds
{
    public class Refund : Entity
    {
        public RefundId Id { get; private init; }
        public PaymentId PaymentId { get; private init; }
        public OrderId OrderId { get; private init; }
        public ExternalRefundId? ExternalRefundId { get; private set; }
        public decimal Amount { get; private init; }
        public string Currency { get; private init; } = null!;
        public RefundStatus Status { get; private set; }
        public string? CancellationParty { get; private set; }
        public string? CancellationReason { get; private set; }
        public string? Description { get; private set; }
        public DateTime CreatedAt { get; private init; }

        public Payment Payment { get; private set; } = null!;

        protected Refund() { }

        public Refund(PaymentId paymentId, OrderId orderId, decimal amount, string currency, string? description = null)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(amount);
            ArgumentNullException.ThrowIfNullOrEmpty(currency);

            Id = RefundId.New();
            PaymentId = paymentId;
            OrderId = orderId;
            Amount = amount;
            Currency = currency;
            Description = description;
            Status = RefundStatus.Pending;
            CreatedAt = DateTime.UtcNow;
        }

        public void ApplyGatewayResult(ExternalRefundId externalRefundId, RefundStatus status,
            string? cancellationParty = null, string? cancellationReason = null)
        {
            if (Status != RefundStatus.Pending)
                return;
            ExternalRefundId = externalRefundId;
            switch (status)
            {
                case RefundStatus.Pending:
                    Status = RefundStatus.Pending;
                    break;

                case RefundStatus.Succeeded:
                    Status = RefundStatus.Succeeded;
                    bool isFullRefund = Payment.RefundedAmount == Amount;
                    Payment.Order.ChangeStatus(isFullRefund ? OrderStatus.Refunded : OrderStatus.PartiallyRefunded);
                    //Возможно не стоит передавать isFullRefund, а сделать развилку на 2 события
                    //Потому что потом будет либо плодиться if/else в обработчике события если нам важен статус в выборе
                    //Не факт, что вообще для дальнейшей обработки нам важно полный возврат или нет
                    AddDomainEvent(new RefundSucceededEvent(Id,PaymentId,OrderId,Amount, isFullRefund));
                    // product.AddToStock
                    break;

                case RefundStatus.Canceled:
                    Status = RefundStatus.Canceled;
                    CancellationParty = cancellationParty;
                    CancellationReason = cancellationReason;
                    break;
            }
            
        }
       
    }
}

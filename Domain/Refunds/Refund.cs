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
        public string ProviderName { get; private set; } = default!;
        public DateTime? NextReconciliationCheckAt { get; private set; }
        public int ReconciliationAttempts { get; private set; }

        public Payment Payment { get; private set; } = null!;

        private readonly List<RefundItem> _items = new();
        public IReadOnlyCollection<RefundItem> Items => _items.AsReadOnly();

        protected Refund() { }

        public void AddItem(Product product, int quantity, decimal unitPrice)
        {
            ArgumentNullException.ThrowIfNull(product);
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(quantity, nameof(quantity));
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(unitPrice, nameof(unitPrice));

            if (_items.Any(i => i.ProductId == product.Id))
                throw new InvalidOperationException($"Товар {product.Id} уже присутствует в возврате");

            _items.Add(new RefundItem(this, product, quantity, unitPrice));
        }

        public Refund(Payment payment, PaymentId paymentId, OrderId orderId, decimal amount, string currency, string providerName, string? description = null)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(amount);
            ArgumentNullException.ThrowIfNullOrEmpty(currency);
            Payment = payment;
            Id = RefundId.New();
            PaymentId = paymentId;
            OrderId = orderId;
            Amount = amount;
            Currency = currency;
            Description = description;
            Status = RefundStatus.Pending;
            CreatedAt = DateTime.UtcNow;
            ProviderName = providerName;
            ScheduleFirstCheck();
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
                    ScheduleNextCheck();
                    break;

                case RefundStatus.Succeeded:
                    Status = RefundStatus.Succeeded;
                    bool isFullRefund = Payment.RefundedAmount == Payment.Amount;
                    Payment.Order.ChangeStatus(isFullRefund ? OrderStatus.Refunded : OrderStatus.PartiallyRefunded);
                    StopReconciliation();
                    AddDomainEvent(new RefundSucceededEvent(DomainEventId.New(), Id, PaymentId, OrderId, Amount, isFullRefund));
                    // product.AddToStock
                    break;

                case RefundStatus.Canceled:
                    Status = RefundStatus.Canceled;
                    CancellationParty = cancellationParty;
                    CancellationReason = cancellationReason;
                    StopReconciliation();
                    break;
            }
            
        }
        private void ScheduleFirstCheck()
        {
            NextReconciliationCheckAt = DateTime.UtcNow + TimeSpan.FromMinutes(2);
        }

        private void ScheduleNextCheck()
        {
            ReconciliationAttempts++;
            var delayMinutes = Math.Min(2 * Math.Pow(2, ReconciliationAttempts), 60);
            NextReconciliationCheckAt = DateTime.UtcNow + TimeSpan.FromMinutes(delayMinutes);
        }

        private void StopReconciliation()
        {
            NextReconciliationCheckAt = null;
        }

    }
}

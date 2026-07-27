using PaymentAPI.Primitives;

namespace PaymentAPI.Models
{
    public class Refund
    {
        public RefundId Id { get; private init; }
        public PaymentId PaymentId { get; private init; }
        public string? ExternalRefundId { get; private set; }
        public decimal Amount { get; private init; }
        public string Currency { get; private init; } = null!;
        public RefundStatus Status { get; private set; }
        public string? CancellationParty { get; private set; }
        public string? CancellationReason { get; private set; }
        public string? Description { get; private set; }
        public DateTime CreatedAt { get; private init; }

        public Payment Payment { get; private set; } = null!;

        protected Refund() { }

        public Refund(PaymentId paymentId, decimal amount, string currency, string? description = null)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(amount);
            ArgumentNullException.ThrowIfNullOrEmpty(currency);

            Id = RefundId.New();
            PaymentId = paymentId;
            Amount = amount;
            Currency = currency;
            Description = description;
            Status = RefundStatus.Pending;
            CreatedAt = DateTime.UtcNow;
        }

        public void ApplyGatewayResult(string externalRefundId, string status,
            string? cancellationParty = null, string? cancellationReason = null)
        {
            ExternalRefundId = externalRefundId;

            if (status == "succeeded")
                Status = RefundStatus.Succeeded;
            else if (status == "canceled")
            {
                Status = RefundStatus.Canceled;
                CancellationParty = cancellationParty;
                CancellationReason = cancellationReason;
            }
        }

        public void SetSucceeded()
        {
            Status = RefundStatus.Succeeded;
        }

        public void SetCanceled(string party, string reason)
        {
            Status = RefundStatus.Canceled;
            CancellationParty = party;
            CancellationReason = reason;
        }
    }
}

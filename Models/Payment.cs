using PaymentAPI.Primitives;

namespace PaymentAPI.Models
{
    public class Payment
    {
        public PaymentId Id { get; private init; }
        public PaymentStatus Status { get; private set; }
        public ExternalPaymentId? ExternalPaymentId { get; private set; }
        public decimal Amount { get; private init; }
        public string Currency { get; private init; } = null!;
        public DateTime CreatedAt { get; private init; }
        public string? Description { get; private set; }

        public OrderId? OrderId { get; private init; }
        public UserId UserId { get; private init; }

        public Order Order { get; private set; } = null!;
        public User User { get; private set; } = null!;
        protected Payment() { }
        public Payment(OrderId? orderId,UserId userId,decimal amount, string currency,
            string? description = null, ExternalPaymentId? externalPaymentId = null)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(amount, nameof(amount));
            ArgumentNullException.ThrowIfNullOrEmpty(currency, nameof(currency));

            Id = PaymentId.New();
            OrderId = orderId;
            UserId = userId;
            ExternalPaymentId = externalPaymentId;
            Amount = amount;
            Currency = currency;
            CreatedAt = DateTime.UtcNow;
            Status = PaymentStatus.Pending;
            Description = description;
        }
        public void ChangeStatus(PaymentStatus status)
        {
            if (Status == status) return;
            Status = status;
        }
    }
}

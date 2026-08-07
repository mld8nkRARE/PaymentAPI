using PaymentAPI.Domain.Primitives;
using PaymentAPI.Domain.Refunds;
using PaymentAPI.Primitives;

namespace PaymentAPI.Domain.Payments
{
    public class Payment : Entity
    {
        public PaymentId Id { get; private init; }
        public PaymentStatus Status { get; private set; }
        public ExternalPaymentId? ExternalPaymentId { get; private set; }
        public decimal Amount { get; private init; }
        public string Currency { get; private init; } = null!;
        public string IdempotencyKey { get; private init; }
        public DateTime CreatedAt { get; private init; }
        public string? Description { get; private set; }
        public string ProviderName { get; private init; }
        public decimal RefundedAmount => Refunds
        .Where(r => r.Status == RefundStatus.Succeeded)
        .Sum(r => r.Amount);

        private List<Refund> _refunds = new();
        public IReadOnlyCollection<Refund> Refunds => _refunds.AsReadOnly();

        public OrderId OrderId { get; private init; }
        public UserId UserId { get; private init; }

        public Order Order { get; private set; } = null!;
        public User User { get; private set; } = null!;
        protected Payment() { }
        public Payment(OrderId orderId, UserId userId, decimal amount, string currency, string idempotencyKey, string providerName,
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
            IdempotencyKey = idempotencyKey;
            CreatedAt = DateTime.UtcNow;
            Status = PaymentStatus.Pending;
            Description = description;
            ProviderName = providerName;
        }

        public void ValidateRefund(decimal amount, string currency, UserId userId)
        {
            if (UserId != userId)
                throw new InvalidOperationException($"Платёж {Id} не принадлежит пользователю");

            if (Status != PaymentStatus.Succeeded)
                throw new InvalidOperationException($"Платёж {Id} не в статусе Succeeded (текущий: {Status})");

            if (ExternalPaymentId is null)
                throw new InvalidOperationException($"Платёж {Id} не имеет ExternalPaymentId");

            var available = Amount - RefundedAmount;


            if (amount <= 0 || amount > available)
                throw new InvalidOperationException($"Недопустимая сумма возврата {amount}. Доступно: {available}");

            if (Order is null)
                throw new InvalidOperationException($"Заказ для платежа {Id} не найден");

            if (Order.Status != OrderStatus.Paid && Order.Status != OrderStatus.PartiallyRefunded)
                throw new InvalidOperationException($"Возврат невозможен: статус заказа {Order.Status}");

            var remaining = available - amount;
            if (remaining > 0 && remaining < 1)
                throw new InvalidOperationException(
                    $"Остаток после возврата {remaining} должен быть >= 1 или 0");
        }

        public Refund RequestRefund(decimal amount, string currency, string? description, UserId userId)
        {
            ValidateRefund(amount, currency, userId);

            var refund = new Refund(this, Id,OrderId, amount, currency, ProviderName, description);
            _refunds.Add(refund);
            return refund;
        }
        public void ApplyGatewayResult(PaymentStatus paymentStatus)
        {
            if (Status == paymentStatus || Status == PaymentStatus.Canceled
                || Status == PaymentStatus.Succeeded)
                return;

            Status = paymentStatus;

            if (Status == PaymentStatus.Succeeded)
            {
                Order.ChangeStatus(OrderStatus.Paid);
                AddDomainEvent(new PaymentSucceededEvent(DomainEventId.New(), Id, OrderId, Amount, Currency));
            }
            else if(Status == PaymentStatus.WaitingForCapture)
                Order.ChangeStatus(OrderStatus.WaitingForCapture);

            else if(Status == PaymentStatus.Canceled)
                Order.ChangeStatus(OrderStatus.Cancelled);
        }
    }
}

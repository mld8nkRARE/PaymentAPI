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
        public string IdempotencyKey { get; private init; }
        public DateTime CreatedAt { get; private init; }
        public string? Description { get; private set; }

        public OrderId OrderId { get; private init; }
        public UserId UserId { get; private init; }

        public Order Order { get; private set; } = null!;
        public User User { get; private set; } = null!;
        protected Payment() { }
        public Payment(OrderId orderId, UserId userId, decimal amount, string currency, string idempotencyKey,
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
        }

        public Refund RequestRefund(decimal amount, string currency, string? description, UserId userId)
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

            //TO DO
            //if (amount < 1 && payment.Currency == "RUB")
            //    throw new InvalidOperationException("Минимальная сумма возврата — 1 рубль");


            if (Order is null)
                throw new InvalidOperationException($"Заказ для платежа {Id} не найден");

            if (Order.Status != OrderStatus.Paid && Order.Status != OrderStatus.PartiallyRefunded)
                throw new InvalidOperationException($"Возврат невозможен: статус заказа {Order.Status}");

            //TO DO
            var remaining = available - amount;
            if (remaining > 0 && remaining < 1)
                throw new InvalidOperationException(
                    $"Остаток после возврата {remaining} должен быть >= 1 или 0");
            //
            var refund = new Refund(Id,OrderId, amount, currency, description);
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
                Order.ChangeStatus(OrderStatus.Paid);

            else if(Status == PaymentStatus.WaitingForCapture)
                Order.ChangeStatus(OrderStatus.WaitingForCapture);

            else if(Status == PaymentStatus.Canceled)
                Order.ChangeStatus(OrderStatus.Cancelled);
        }
    }
}

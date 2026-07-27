using PaymentAPI.Infrastructure;
using PaymentAPI.Models;
using PaymentAPI.Primitives;
using Microsoft.EntityFrameworkCore;

namespace PaymentAPI.Services
{
    public class RefundValidator
    {
        private readonly ApplicationDbContext _db;

        public RefundValidator(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task ValidateAsync(Payment payment, decimal amount, UserId userId)
        {
            if (payment.UserId != userId)
                throw new InvalidOperationException($"Платёж {payment.Id} не принадлежит пользователю");

            if (payment.Status != PaymentStatus.Succeeded)
                throw new InvalidOperationException($"Платёж {payment.Id} не в статусе Succeeded (текущий: {payment.Status})");

            if (payment.ExternalPaymentId is null)
                throw new InvalidOperationException($"Платёж {payment.Id} не имеет ExternalPaymentId");

            if (amount <= 0)
                throw new InvalidOperationException("Сумма возврата должна быть положительной");

            var alreadyRefunded = await _db.Refunds
                .Where(r => r.PaymentId == payment.Id && r.Status == RefundStatus.Succeeded)
                .SumAsync(r => r.Amount);

            var available = payment.Amount - alreadyRefunded;

            if (amount > available)
                throw new InvalidOperationException(
                    $"Сумма возврата {amount} превышает доступную сумму {available}");

            if (amount < 1)
                throw new InvalidOperationException("Минимальная сумма возврата — 1 рубль");

            var remaining = available - amount;
            if (remaining > 0 && remaining < 1)
                throw new InvalidOperationException(
                    $"После возврата {amount} от суммы {available} останется {remaining}. " +
                    "Остаток должен быть >= 1 рубля или 0 рублей.");

            var order = await _db.Orders
                .FirstOrDefaultAsync(o => o.Id == payment.OrderId);

            if (order is null)
                throw new InvalidOperationException($"Заказ для платежа {payment.Id} не найден");

            if (order.Status != OrderStatus.Paid && order.Status != OrderStatus.PartiallyRefunded)
                throw new InvalidOperationException(
                    $"Заказ {order.Id} в статусе {order.Status}. Возврат возможен только для Paid или PartiallyRefunded");
        }
    }
}
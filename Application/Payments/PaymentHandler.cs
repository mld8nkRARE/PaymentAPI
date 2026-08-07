using Microsoft.EntityFrameworkCore;
using PaymentAPI.Domain.Payments;
using PaymentAPI.Domain.Primitives;
using PaymentAPI.DTO.payment;
using PaymentAPI.Infrastructure;
using PaymentAPI.Primitives;
using PaymentAPI.Providers.Interfaces;

namespace PaymentAPI.Application.Payments
{
    public class PaymentHandler
    {
        private readonly IEnumerable<IPaymentGateway> _gateways;
        private readonly ApplicationDbContext _db;

        public PaymentHandler(IEnumerable<IPaymentGateway> gateways, ApplicationDbContext db)
        {
            _gateways = gateways;
            _db = db;
        }

        public async Task<PaymentResult> CreatePaymentAsync(PaymentCreateRequest request, UserId userId, string idempotenceKey)
        {
            var order = await _db.Orders
                .FirstOrDefaultAsync(o => o.Id == request.OrderId && o.UserId == userId)
                ?? throw new InvalidOperationException($"Заказ {request.OrderId} не найден");

            if (order.Status != OrderStatus.Pending)
                throw new InvalidOperationException($"Создание платежа возможно только для заказа в статусе Pending (текущий: {order.Status})");

            var command = request.ToCommand(order.TotalPrice);

            var gateway = _gateways.FirstOrDefault(g => g.ProviderName.Equals(command.ProviderName, StringComparison.OrdinalIgnoreCase))
                ?? throw new NotSupportedException($"Провайдер {command.ProviderName} не поддерживается");

            var externalResult = await gateway.CreatePaymentAsync(command, idempotenceKey);

            var description = request.Description;
            var orderId = request.OrderId;

            var payment = new Payment(
                orderId,
                userId,
                order.TotalPrice,
                externalResult.Currency,
                idempotenceKey,
                command.ProviderName,
                description,
                externalResult.ExternalPaymentId);

            _db.Payments.Add(payment);
            await _db.SaveChangesAsync();

            return externalResult;
        }
    }
}

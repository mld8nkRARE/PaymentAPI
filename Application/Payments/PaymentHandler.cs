using PaymentAPI.Domain.Payments;
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
            var command = request.ToCommand();
            
            var gateway = _gateways.FirstOrDefault(g => g.ProviderName.Equals(command.ProviderName, StringComparison.OrdinalIgnoreCase))
                ?? throw new NotSupportedException($"Провайдер {command.ProviderName} не поддерживается");

            var externalResult = await gateway.CreatePaymentAsync(command, idempotenceKey);
            
            var description = request.Description;
            var orderId = request.OrderId;

            var payment = new Payment(
                orderId,
                userId,
                externalResult.Amount,
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

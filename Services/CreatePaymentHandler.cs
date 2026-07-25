using PaymentAPI.DTO;
using PaymentAPI.Infrastructure;
using PaymentAPI.Interfaces;
using PaymentAPI.Primitives;
using PaymentAPI.Models;

namespace PaymentAPI.Services
{
    public class CreatePaymentHandler
    {
        private readonly Dictionary<string, IPaymentGateway> _gateways;
        private readonly ApplicationDbContext _db;

        public CreatePaymentHandler(IEnumerable<IPaymentGateway> gateways, ApplicationDbContext db)
        {
            _gateways = gateways.ToDictionary(g => g.ProviderName, StringComparer.OrdinalIgnoreCase);
            _db = db;
        }

        public async Task<PaymentResult> HandleAsync(CreatePaymentRequest request, UserId userId, string idempotenceKey)
        {
            if (!_gateways.TryGetValue(request.Provider, out var gateway))
                throw new NotSupportedException($"Провайдер {request.Provider} не поддерживается");

            var externalResult = await gateway.CreatePayment(request.PaymentData, idempotenceKey);

            var description = request.PaymentData.TryGetProperty("description", out var d)
                ? d.GetString() : null;

            var payment = new Payment(
                request.OrderId,
                userId,
                externalResult.Amount,
                externalResult.Currency,
                description,
                new ExternalPaymentId(externalResult.ExternalPaymentId));

            _db.Payments.Add(payment);
            await _db.SaveChangesAsync();

            return externalResult;
        }
    }
}

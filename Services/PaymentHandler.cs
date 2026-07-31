using PaymentAPI.DTO.payment;
using PaymentAPI.Infrastructure;
using PaymentAPI.Interfaces;
using PaymentAPI.Models;
using PaymentAPI.Primitives;

namespace PaymentAPI.Services
{
    public class PaymentHandler
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ApplicationDbContext _db;

        public PaymentHandler(IServiceProvider ServiceProvider, ApplicationDbContext db)
        {
            _serviceProvider = ServiceProvider;
            _db = db;
        }

        public async Task<PaymentResult> CreatePaymentAsync(PaymentCreateRequest request, UserId userId, string idempotenceKey)
        {
            var command = request.ToCommand();
            var gateway = ResolveGateway(command);
            var externalResult = await ((dynamic)gateway).CreatePaymentAsync((dynamic)command, idempotenceKey);

            var description = request.Description;
            var orderId = request.OrderId;

            var payment = new Payment(
                orderId,
                userId,
                externalResult.Amount,
                externalResult.Currency,
                idempotenceKey,
                description,
                externalResult.ExternalPaymentId);

            _db.Payments.Add(payment);
            await _db.SaveChangesAsync();

            return externalResult;
        }
        private object ResolveGateway(PaymentCreateCommand command)
        {
            var gatewayProviderType = typeof(IPaymentGateway<>).MakeGenericType(command.GetType());
            var gateway = _serviceProvider.GetService(gatewayProviderType);

            if (gateway is null)
                throw new NotSupportedException($"Провайдер для {command.GetType().Name} не поддерживается");

            return gateway;
        }
    }
}

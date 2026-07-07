using Microsoft.Extensions.Options;
using PaymentAPI.DTO;
using PaymentAPI.Interfaces;
using PaymentAPI.Settings;
using Yandex.Checkout.V3;
using PaymentAPI.Primitives;
namespace PaymentAPI.Services
{
    public class YookassaGateway : IPaymentGateway
    {
        private readonly YookassaSettings _yookassaSettings;
        private readonly AsyncClient _client;
        public YookassaGateway(IOptions<YookassaSettings> yookassaSettings, IHttpClientFactory httpClientFactory)
        {
            _yookassaSettings = yookassaSettings.Value;
            var client = new Client(yookassaSettings.Value.ShopId, yookassaSettings.Value.SecretKey);
            var httpClient = httpClientFactory.CreateClient();
            _client = new AsyncClient(httpClient, false, client);
        }
        public async Task<PaymentResult> CreatePayment(PaymentRequest request, string idempotenceKey)
        {
            NewPayment newPayment = new NewPayment()
            {
                Amount = request.Amount,
                Description = request.Description,
                Confirmation = new Confirmation
                {
                    Type = ConfirmationType.Redirect,
                    ReturnUrl = _yookassaSettings.ReturnUrl
                }
            };
            Yandex.Checkout.V3.Payment payment = await _client.CreatePaymentAsync(newPayment,idempotenceKey);
            var paymentId = new PaymentId(Guid.Parse(payment.Id));
            
            PaymentResult result = new PaymentResult
            {
                Status = payment.Status,
                PaymentId = paymentId,
                Amount = payment.Amount,
                ConfirmationUrl = payment.Confirmation.ConfirmationUrl,
                CreatedAt = payment.CreatedAt
            };
            return result;
        }
        

    }
}

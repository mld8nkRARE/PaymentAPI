using Microsoft.Extensions.Options;
using PaymentAPI.DTO;
using PaymentAPI.Interfaces;
using PaymentAPI.Settings;
using System.Text.Json;
using Yandex.Checkout.V3;

namespace PaymentAPI.Services
{
    public class YookassaGateway : IPaymentGateway
    {
        public string ProviderName => "yookassa";
        private readonly YookassaSettings _yookassaSettings;
        private readonly AsyncClient _client;

        public YookassaGateway(IOptions<YookassaSettings> yookassaSettings, IHttpClientFactory httpClientFactory)
        {
            _yookassaSettings = yookassaSettings.Value;
            var client = new Client(yookassaSettings.Value.ShopId, yookassaSettings.Value.SecretKey);
            var httpClient = httpClientFactory.CreateClient();
            _client = new AsyncClient(httpClient, false, client);
        }

        public async Task<PaymentResult> CreatePayment(JsonElement paymentData, string idempotenceKey)
        {
            if (!paymentData.TryGetProperty("amount", out var amountElement))
                throw new ArgumentException("Отсутствует поле amount", nameof(paymentData));
            if (!paymentData.TryGetProperty("currency", out var currencyElement))
                throw new ArgumentException("Отсутствует поле currency", nameof(paymentData));

            var amount = amountElement.GetDecimal();
            var currency = currencyElement.GetString()
                ?? throw new ArgumentException("Поле currency не может быть null", nameof(paymentData));
            var description = paymentData.TryGetProperty("description", out var d)
                ? d.GetString() : null;

            NewPayment newPayment = new NewPayment()
            {
                Amount = new Amount
                {
                    Value = amount,
                    Currency = currency
                },
                Description = description,
                Confirmation = new Confirmation
                {
                    Type = ConfirmationType.Redirect,
                    ReturnUrl = _yookassaSettings.ReturnUrl
                }
            };

            Yandex.Checkout.V3.Payment payment = await _client.CreatePaymentAsync(newPayment, idempotenceKey);
            var get = await _client.GetPaymentAsync(payment.Id);
            
            return new PaymentResult
            {
                PaymentId = string.Empty,
                Status = payment.Status.ToString(),
                ExternalPaymentId = payment.Id,
                Amount = payment.Amount.Value,
                Currency = payment.Amount.Currency,
                ConfirmationUrl = payment.Confirmation.ConfirmationUrl,
                CreatedAt = payment.CreatedAt
            };
        }

        public async Task<PaymentWebhookResult> HandleWebhookAsync(JsonElement webhookBody)
        {
            var id = webhookBody.GetProperty("object").GetProperty("id").GetString()
                ?? throw new ArgumentException("Отсутствует id в webhook");

            var paymentFromApi = await _client.GetPaymentAsync(id);
            var status = (PaymentAPI.Primitives.PaymentStatus)(int)paymentFromApi.Status;
            return new PaymentWebhookResult(id, status);
        }
    }
}

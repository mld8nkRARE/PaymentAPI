using Microsoft.Extensions.Options;
using PaymentAPI.DTO.payment;
using PaymentAPI.Interfaces;
using PaymentAPI.Primitives;
using PaymentAPI.Settings;
using System.Text.Json;
using Yandex.Checkout.V3;

namespace PaymentAPI.Services
{
    public class YookassaPaymentWebhookHandler : IPaymentWebhookHandler
    {
        public string ProviderName => "yookassa";
        private readonly AsyncClient _client;
        public YookassaPaymentWebhookHandler(IOptions<YookassaSettings> yookassaSettings, IHttpClientFactory httpClientFactory)
        {
            var client = new Client(yookassaSettings.Value.ShopId, yookassaSettings.Value.SecretKey);
            var httpClient = httpClientFactory.CreateClient();
            _client = new AsyncClient(httpClient, false, client);
        }

        public async Task<PaymentWebhookResult> HandlePaymentWebhookAsync(JsonElement webhookBody)
        {
            var id = webhookBody.GetProperty("object").GetProperty("id").GetString()
                ?? throw new ArgumentException("Отсутствует id в webhook");

            var paymentFromApi = await _client.GetPaymentAsync(id);
            var status = paymentFromApi.Status switch
            {
                Yandex.Checkout.V3.PaymentStatus.Pending => PaymentAPI.Primitives.PaymentStatus.Pending,
                Yandex.Checkout.V3.PaymentStatus.WaitingForCapture => PaymentAPI.Primitives.PaymentStatus.WaitingForCapture,
                Yandex.Checkout.V3.PaymentStatus.Succeeded => PaymentAPI.Primitives.PaymentStatus.Succeeded,
                Yandex.Checkout.V3.PaymentStatus.Canceled => PaymentAPI.Primitives.PaymentStatus.Canceled,
                _ => throw new NotSupportedException($"Неизвестный статус платежа от Yookassa: {paymentFromApi.Status}")
            };
            return new PaymentWebhookResult(new ExternalPaymentId(id), status);
        }
    }
}

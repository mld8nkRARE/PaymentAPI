using Microsoft.Extensions.Options;
using PaymentAPI.DTO.payment;
using PaymentAPI.Primitives;
using PaymentAPI.Providers.Interfaces;
using System.Text.Json;
using Yandex.Checkout.V3;

namespace PaymentAPI.Providers.Yookassa
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
                Yandex.Checkout.V3.PaymentStatus.Pending => Domain.Primitives.PaymentStatus.Pending,
                Yandex.Checkout.V3.PaymentStatus.WaitingForCapture => Domain.Primitives.PaymentStatus.WaitingForCapture,
                Yandex.Checkout.V3.PaymentStatus.Succeeded => Domain.Primitives.PaymentStatus.Succeeded,
                Yandex.Checkout.V3.PaymentStatus.Canceled => Domain.Primitives.PaymentStatus.Canceled,
                _ => throw new NotSupportedException($"Неизвестный статус платежа от Yookassa: {paymentFromApi.Status}")
            };
            return new PaymentWebhookResult(new ExternalPaymentId(id), status);
        }
    }
}

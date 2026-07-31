using Microsoft.Extensions.Options;
using PaymentAPI.DTO.refund;
using PaymentAPI.Interfaces;
using PaymentAPI.Primitives;
using PaymentAPI.Settings;
using System.Text.Json;
using Yandex.Checkout.V3;

namespace PaymentAPI.Services
{
    public class YookassaRefundWebhookHandler : IRefundWebhookHandler
    {
        public string ProviderName => "yookassa";
        private readonly AsyncClient _client;

        public YookassaRefundWebhookHandler(IOptions<YookassaSettings> yookassaSettings, IHttpClientFactory httpClientFactory)
        {
            var client = new Client(yookassaSettings.Value.ShopId, yookassaSettings.Value.SecretKey);
            var httpClient = httpClientFactory.CreateClient();
            _client = new AsyncClient(httpClient, false, client);
        }
        public async Task<RefundWebhookResult> HandleRefundWebhookAsync(JsonElement webhookBody)
        {
            var id = webhookBody.GetProperty("object").GetProperty("id").GetString()
                ?? throw new ArgumentException("Отсутствует id в webhook");

            var refundFromApi = await _client.GetRefundAsync(id);
            var status = refundFromApi.Status switch
            {
                Yandex.Checkout.V3.RefundStatus.Pending => PaymentAPI.Primitives.RefundStatus.Pending,
                Yandex.Checkout.V3.RefundStatus.Succeeded => PaymentAPI.Primitives.RefundStatus.Succeeded,
                Yandex.Checkout.V3.RefundStatus.Canceled => PaymentAPI.Primitives.RefundStatus.Canceled,
                _ => throw new NotSupportedException($"Неизвестный статус платежа от Yookassa: {refundFromApi.Status}")
            };
            return new RefundWebhookResult(new ExternalRefundId(id), status);
        }
    }
}

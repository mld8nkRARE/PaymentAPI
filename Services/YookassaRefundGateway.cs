using Microsoft.Extensions.Options;
using PaymentAPI.DTO;
using PaymentAPI.Interfaces;
using PaymentAPI.Settings;
using System.Text.Json;
using Yandex.Checkout.V3;

namespace PaymentAPI.Services
{
    public class YookassaRefundGateway : IRefundGateway
    {
        public string ProviderName => "yookassa";
        private readonly AsyncClient _client;

        public YookassaRefundGateway(IOptions<YookassaSettings> yookassaSettings, IHttpClientFactory httpClientFactory)
        {
            var client = new Client(yookassaSettings.Value.ShopId, yookassaSettings.Value.SecretKey);
            var httpClient = httpClientFactory.CreateClient();
            _client = new AsyncClient(httpClient, false, client);
        }

        public async Task<RefundResult> CreateRefundAsync(string paymentId, decimal amount, string currency, string idempotenceKey)
        {
            var newRefund = new NewRefund
            {
                PaymentId = paymentId,
                Amount = new Amount
                {
                    Value = amount,
                    Currency = currency
                }
            };

            var yookassaRefund = await _client.CreateRefundAsync(newRefund, idempotenceKey);

            return MapToRefundResult(yookassaRefund);
        }

        public async Task<RefundWebhookResult> HandleRefundWebhookAsync(JsonElement webhookBody)
        {
            var id = webhookBody.GetProperty("object").GetProperty("id").GetString()
                ?? throw new ArgumentException("Отсутствует id в webhook");

            var refundFromApi = await _client.GetRefundAsync(id);
            var status = refundFromApi.Status.ToString().ToLower();

            return new RefundWebhookResult(id, status);
        }

        public async Task<RefundResult> GetRefundAsync(string refundId)
        {
            var yookassaRefund = await _client.GetRefundAsync(refundId);
            return MapToRefundResult(yookassaRefund);
        }

        private static RefundResult MapToRefundResult(Yandex.Checkout.V3.Refund yookassaRefund)
        {
            string? cancellationParty = null;
            string? cancellationReason = null;

            if (yookassaRefund.CancellationDetails is not null)
            {
                cancellationParty = yookassaRefund.CancellationDetails.Party;
                cancellationReason = yookassaRefund.CancellationDetails.Reason;
            }

            return new RefundResult(
                yookassaRefund.Id,
                yookassaRefund.Amount.Value,
                yookassaRefund.Amount.Currency,
                yookassaRefund.Status.ToString().ToLower(),
                cancellationParty,
                cancellationReason);
        }
    }
}

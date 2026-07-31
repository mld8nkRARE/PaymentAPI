using Microsoft.Extensions.Options;
using PaymentAPI.Domain.Refund;
using PaymentAPI.DTO.refund;
using PaymentAPI.Primitives;
using PaymentAPI.Providers.Interfaces;
using System.Text.Json;
using Yandex.Checkout.V3;

namespace PaymentAPI.Providers.Yookassa
{
    public class YookassaRefundGateway : IRefundGateway<RefundCreateYookassaCommand>
    {
        public string ProviderName => "yookassa";
        private readonly AsyncClient _client;

        public YookassaRefundGateway(IOptions<YookassaSettings> yookassaSettings, IHttpClientFactory httpClientFactory)
        {
            var client = new Client(yookassaSettings.Value.ShopId, yookassaSettings.Value.SecretKey);
            var httpClient = httpClientFactory.CreateClient();
            _client = new AsyncClient(httpClient, false, client);
        }

        public async Task<RefundResult> CreateRefundAsync(RefundCreateYookassaCommand cmd, string idempotenceKey)
        {
            var newRefund = new NewRefund
            {
                PaymentId = cmd.PaymentId.ToString(),
                Amount = new Amount
                {
                    Value = cmd.Amount,
                    Currency = cmd.Currency
                }
            };

            var yookassaRefund = await _client.CreateRefundAsync(newRefund, idempotenceKey);

            return MapToRefundResult(yookassaRefund);
        }

        public async Task<RefundResult> GetRefundAsync(string refundId)
        {
            var refundFromApi = await _client.GetRefundAsync(refundId);
            return MapToRefundResult(refundFromApi);
        }

        private static RefundResult MapToRefundResult(Yandex.Checkout.V3.Refund refundFromApi)
        {
            string? cancellationParty = null;
            string? cancellationReason = null;

            if (refundFromApi.CancellationDetails is not null)
            {
                cancellationParty = refundFromApi.CancellationDetails.Party;
                cancellationReason = refundFromApi.CancellationDetails.Reason;
            }
            var status = refundFromApi.Status switch
            {
                Yandex.Checkout.V3.RefundStatus.Pending => Domain.Primitives.RefundStatus.Pending,
                Yandex.Checkout.V3.RefundStatus.Succeeded => Domain.Primitives.RefundStatus.Succeeded,
                Yandex.Checkout.V3.RefundStatus.Canceled => Domain.Primitives.RefundStatus.Canceled,
                _ => throw new NotSupportedException($"Неизвестный статус платежа от Yookassa: {refundFromApi.Status}")
            };
            return new RefundResult(
                new ExternalRefundId(refundFromApi.Id),
                new ExternalPaymentId(refundFromApi.PaymentId),
                refundFromApi.Amount.Value,
                refundFromApi.Amount.Currency,
                status,
                cancellationParty,
                cancellationReason);
        }
    }
}

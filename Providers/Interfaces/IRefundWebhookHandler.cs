using PaymentAPI.DTO.refund;
using System.Text.Json;

namespace PaymentAPI.Providers.Interfaces
{
    public interface IRefundWebhookHandler
    {
        string ProviderName { get; }
        Task<RefundWebhookResult> HandleRefundWebhookAsync(JsonElement webhookBody);
    }
}

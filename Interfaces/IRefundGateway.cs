using PaymentAPI.DTO;
using System.Text.Json;

namespace PaymentAPI.Interfaces
{
    public interface IRefundGateway
    {
        string ProviderName { get; }
        Task<RefundResult> CreateRefundAsync(string paymentId, decimal amount, string currency, string idempotenceKey);
        Task<RefundWebhookResult> HandleRefundWebhookAsync(JsonElement webhookBody);
        Task<RefundResult> GetRefundAsync(string refundId);
    }
}

using PaymentAPI.DTO.payment;
using System.Text.Json;

namespace PaymentAPI.Interfaces
{
    public interface IPaymentWebhookHandler
    {
        string ProviderName { get; }
        Task<PaymentWebhookResult> HandlePaymentWebhookAsync(JsonElement webhookBody);
    }
}

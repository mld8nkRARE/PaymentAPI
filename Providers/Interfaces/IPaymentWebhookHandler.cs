using PaymentAPI.DTO.payment;
using System.Text.Json;

namespace PaymentAPI.Providers.Interfaces
{
    public interface IPaymentWebhookHandler
    {
        string ProviderName { get; }
        Task<PaymentWebhookResult> HandlePaymentWebhookAsync(JsonElement webhookBody);
    }
}

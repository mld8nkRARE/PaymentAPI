using PaymentAPI.DTO;
using System.Text.Json;

namespace PaymentAPI.Interfaces
{
    public interface IPaymentGateway
    {
        string ProviderName { get; }
        Task<PaymentResult> CreatePayment(JsonElement paymentData, string idempotenceKey);
        Task<WebhookResult> HandleWebhookAsync(JsonElement webhookBody);
    }
}

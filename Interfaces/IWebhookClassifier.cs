using PaymentAPI.Primitives;
using System.Text.Json;

namespace PaymentAPI.Interfaces
{
    public interface IWebhookClassifier
    {
        string ProviderName { get; }
        WebhookType GetWebhookType(JsonElement webhookBody);
    }
}

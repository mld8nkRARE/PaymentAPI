using PaymentAPI.Domain.Primitives;
using System.Text.Json;

namespace PaymentAPI.Providers.Interfaces
{
    public interface IWebhookClassifier
    {
        string ProviderName { get; }
        WebhookType GetWebhookType(JsonElement webhookBody);
    }
}

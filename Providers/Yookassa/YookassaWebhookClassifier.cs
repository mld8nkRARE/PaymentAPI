using PaymentAPI.Domain.Primitives;
using PaymentAPI.Providers.Interfaces;
using System.Text.Json;

namespace PaymentAPI.Providers.Yookassa
{
    public class YookassaWebhookClassifier : IWebhookClassifier
    {
       public string ProviderName => "yookassa";
       public WebhookType GetWebhookType(JsonElement webhookBody)
       {
            var eventType = webhookBody.GetProperty("event").GetString();
            return eventType switch
            {
                string e when e.StartsWith("payment.") => WebhookType.Payment,
                string e when e.StartsWith("refund.") => WebhookType.Refund,
                string e when e.StartsWith("payout.") => WebhookType.Payout,
                string e when e.StartsWith("deal.") => WebhookType.Deal,
                _ => throw new NotSupportedException($"Неизвестный тип события {eventType}")
            };
       }
    }
}

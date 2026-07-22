using PaymentAPI.DTO;
using PaymentAPI.Interfaces;
using System.Text.Json;

namespace PaymentAPI.Services
{
    public class YookassaWebhookParser : IWebhookParser
    {
        public string ProviderName => "yookassa";
        public PaymentWebhookRequest Parse(JsonElement paymentObject)
        {
            if(!paymentObject.TryGetProperty("event", out var eventType))
            {
                throw new ArgumentException("Не удалось распознать событие", nameof(paymentObject));
            }
            var eventString = eventType.GetString();
            ArgumentNullException.ThrowIfNullOrEmpty(eventString, nameof(paymentObject));
            return new PaymentWebhookRequest(ProviderName, eventString, paymentObject);
        }
    }
}

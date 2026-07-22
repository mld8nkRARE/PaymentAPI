using PaymentAPI.DTO;
using PaymentAPI.Interfaces;
using System.Text.Json;

namespace PaymentAPI.Services
{
    public class WebhookParserContext
    {
        private readonly Dictionary<string, IWebhookParser> _parsers;
        public WebhookParserContext(IEnumerable<IWebhookParser> parsers)
        {
            _parsers = parsers.ToDictionary(k => k.ProviderName, StringComparer.OrdinalIgnoreCase);
        }
        public PaymentWebhookRequest Parse(string provider, JsonElement paymentObject)
        {
            if (_parsers.TryGetValue(provider, out var parser))
            {
                return parser.Parse(paymentObject);
            }
            else
            {
                throw new ArgumentException("Провайдер неизвестен или не распознан", nameof(provider));
            }
        }
    }
}

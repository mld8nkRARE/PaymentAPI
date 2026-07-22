using PaymentAPI.DTO;
using System.Text.Json;

namespace PaymentAPI.Interfaces
{
    public interface IWebhookParser
    {
        string ProviderName { get; }
        PaymentWebhookRequest Parse(JsonElement paymentObject);
    }
}

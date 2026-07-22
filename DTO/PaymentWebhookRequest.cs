using System.Text.Json;

namespace PaymentAPI.DTO
{
    public record PaymentWebhookRequest(string Provider, string Event, JsonElement PaymentObject);
}

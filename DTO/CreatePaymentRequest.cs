using PaymentAPI.Primitives;
using System.Text.Json;

namespace PaymentAPI.DTO
{
    public record CreatePaymentRequest
    {
        public required string Provider { get; init; }
        public required JsonElement PaymentData { get; init; }
        public OrderId? OrderId { get; init; }
    }
}

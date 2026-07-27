using PaymentAPI.Primitives;
using System.Text.Json;

namespace PaymentAPI.DTO
{
    public record PaymentCreateRequest
    {
        public required string Provider { get; init; }
        public required JsonElement PaymentData { get; init; }
        public required OrderId OrderId { get; init; }
    }
}
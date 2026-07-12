using Yandex.Checkout.V3;
namespace PaymentAPI.DTO
{
    public record PaymentRequest
    {
        public required decimal Amount { get; init; }
        public required string Currency { get; init; }
        public required string? Description { get; init; }
    }
}

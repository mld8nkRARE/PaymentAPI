using PaymentAPI.Primitives;

namespace PaymentAPI.DTO.payment
{
    public record PaymentResult
    {
        public required string PaymentId { get; init; }
        public required string Status { get; init; }
        public required string ExternalPaymentId { get; init; }
        public required decimal Amount { get; init; }
        public required string Currency { get; init; }
        public required string? ConfirmationUrl { get; init; }
        public required DateTime CreatedAt { get; init; }
    }
}

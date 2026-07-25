using PaymentAPI.Primitives;

namespace PaymentAPI.DTO
{
    public record WebhookResult(string ExternalPaymentId, PaymentStatus Status);
}

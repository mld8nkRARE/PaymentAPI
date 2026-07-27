using PaymentAPI.Primitives;

namespace PaymentAPI.DTO
{
    public record PaymentWebhookResult(string ExternalPaymentId, PaymentStatus Status);
}
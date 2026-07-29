using PaymentAPI.Primitives;

namespace PaymentAPI.DTO.payment
{
    public record PaymentWebhookResult(string ExternalPaymentId, PaymentStatus Status);
}
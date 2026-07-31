using PaymentAPI.Primitives;

namespace PaymentAPI.DTO.payment
{
    public record PaymentWebhookResult(ExternalPaymentId ExternalPaymentId, PaymentStatus Status);
}
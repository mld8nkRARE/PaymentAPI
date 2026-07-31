using PaymentAPI.Primitives;

namespace PaymentAPI.DTO.refund
{
    public record RefundWebhookResult(ExternalRefundId ExternalRefundId, RefundStatus Status);
}

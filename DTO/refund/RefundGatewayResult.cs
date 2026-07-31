using PaymentAPI.Primitives;

namespace PaymentAPI.DTO.refund
{
    public record RefundResult(
        ExternalRefundId ExternalRefundId,
        ExternalPaymentId ExternalPaymentId,
        decimal Amount,
        string Currency,
        RefundStatus Status,
        string? CancellationParty = null,
        string? CancellationReason = null);

}
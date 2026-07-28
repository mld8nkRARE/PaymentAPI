using PaymentAPI.Primitives;

namespace PaymentAPI.DTO.refund
{
    public record RefundResponse(RefundId Id, PaymentId PaymentId, decimal Amount, string Currency, string Status, DateTime CreatedAt);
}

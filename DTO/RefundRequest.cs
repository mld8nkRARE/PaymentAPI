using PaymentAPI.Primitives;

namespace PaymentAPI.DTO
{
    public record RefundCreateRequest(PaymentId PaymentId, decimal Amount, string Currency, string? Description = null);
    public record RefundResponse(RefundId Id, PaymentId PaymentId, decimal Amount, string Currency, string Status, DateTime CreatedAt);
}
using PaymentAPI.Primitives;

namespace PaymentAPI.Domain.Refund
{
    public record RefundCanceledEvent(
    RefundId RefundId,
    PaymentId PaymentId,
    string? CancellationReason,
    string? CancellationParty) : IDomainEvent;
}

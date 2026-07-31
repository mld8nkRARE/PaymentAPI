using PaymentAPI.Primitives;

namespace PaymentAPI.Domain.Refunds
{
    public record RefundCanceledEvent(
    RefundId RefundId,
    PaymentId PaymentId,
    string? CancellationReason,
    string? CancellationParty) : IDomainEvent;
}

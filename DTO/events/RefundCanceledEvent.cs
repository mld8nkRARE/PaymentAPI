using PaymentAPI.Interfaces;
using PaymentAPI.Primitives;

namespace PaymentAPI.DTO.events
{
    public record RefundCanceledEvent(
    RefundId RefundId,
    PaymentId PaymentId,
    string? CancellationReason,
    string? CancellationParty) : IDomainEvent;
}

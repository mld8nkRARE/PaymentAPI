using MediatR;
using PaymentAPI.Primitives;
using PaymentAPI.Domain.Refunds;

namespace PaymentAPI.Domain.Refunds
{
    public record RefundCanceledEvent(
    DomainEventId EventId,
    RefundId RefundId,
    PaymentId PaymentId,
    string? CancellationReason,
    string? CancellationParty) : IDomainEvent;
}

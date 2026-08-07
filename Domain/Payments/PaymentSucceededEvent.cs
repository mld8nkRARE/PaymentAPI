using MediatR;
using PaymentAPI.Primitives;
using PaymentAPI.Domain.Payments;

namespace PaymentAPI.Domain.Payments
{
    public record PaymentSucceededEvent(
    DomainEventId EventId,
    PaymentId PaymentId,
    OrderId OrderId,
    decimal Amount,
    string Currency) : IDomainEvent;
}

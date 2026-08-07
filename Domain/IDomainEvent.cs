using MediatR;
using PaymentAPI.Primitives;

namespace PaymentAPI.Domain
{
    public interface IDomainEvent : INotification {
        DomainEventId EventId { get; }
    }
}

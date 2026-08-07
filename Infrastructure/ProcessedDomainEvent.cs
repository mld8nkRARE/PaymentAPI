using PaymentAPI.Primitives;

namespace PaymentAPI.Infrastructure
{
    public class ProcessedDomainEvent
    {
        public DomainEventId EventId { get; init; }
        public DateTime ProcessedAt { get; init; } = DateTime.UtcNow;
    }
}

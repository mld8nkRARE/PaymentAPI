using Microsoft.EntityFrameworkCore.Diagnostics;
using PaymentAPI.Domain;
using System.Text.Json;

namespace PaymentAPI.Infrastructure
{
    public class DomainEventPublishingInterceptor : SaveChangesInterceptor
    {
        public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
            DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
        {
            var context = eventData.Context;
            if (context is null) return await base.SavingChangesAsync(eventData, result, cancellationToken);

            var domainEntities = context.ChangeTracker
                .Entries<Entity>()
                .Where(x => x.Entity.DomainEvents != null && x.Entity.DomainEvents.Any())
                .ToList();

            var outboxMessages = domainEntities
                .SelectMany(x => x.Entity.DomainEvents)
                .Select(domainEvent => new OutboxMessage
                {
                    Id = Guid.NewGuid(),
                    Type = domainEvent.GetType().AssemblyQualifiedName!,
                    Content = JsonSerializer.Serialize(domainEvent, domainEvent.GetType()),
                    OccurredOn = DateTime.UtcNow
                })
                .ToList();

            foreach (var entry in domainEntities)
                entry.Entity.ClearDomainEvents();

            if (outboxMessages.Any())
                context.Set<OutboxMessage>().AddRange(outboxMessages);

            return await base.SavingChangesAsync(eventData, result, cancellationToken);
        }
    }
}

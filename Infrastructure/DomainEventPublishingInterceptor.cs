using MediatR;
using Microsoft.EntityFrameworkCore.Diagnostics;
using PaymentAPI.Domain;

namespace PaymentAPI.Infrastructure
{
    public class DomainEventPublishingInterceptor : SaveChangesInterceptor
    {
        private readonly IPublisher _publisher;
        public DomainEventPublishingInterceptor(IPublisher publisher) => _publisher = publisher;

        public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
            DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
        {
            var context = eventData.Context;
            if (context is null) return await base.SavingChangesAsync(eventData, result, cancellationToken);

            while (true)
            {
                var domainEntities = context.ChangeTracker
                    .Entries<Entity>()
                    .Where(x => x.Entity.DomainEvents != null && x.Entity.DomainEvents.Any())
                    .ToList();

                if (!domainEntities.Any())
                    break;

                var domainEvents = domainEntities
                    .SelectMany(x => x.Entity.DomainEvents)
                    .ToList();

                foreach (var entry in domainEntities)
                    entry.Entity.ClearDomainEvents();

                foreach (var domainEvent in domainEvents)
                    await _publisher.Publish(domainEvent, cancellationToken);
            }

            return await base.SavingChangesAsync(eventData, result, cancellationToken);
        }
    }
}

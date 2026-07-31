using MediatR;
using Microsoft.EntityFrameworkCore.Diagnostics;
using PaymentAPI.Models;

namespace PaymentAPI.Services
{
    public class DomainEventPublishingInterceptor : SaveChangesInterceptor
    {
        private readonly IPublisher _publisher;
        public DomainEventPublishingInterceptor(IPublisher publisher) => _publisher = publisher;

        public override async ValueTask<int> SavedChangesAsync(
            SaveChangesCompletedEventData eventData, int result, CancellationToken cancellationToken = default)
        {
            var context = eventData.Context;
            if (context is null) return result;

            var domainEvents = context.ChangeTracker
                .Entries<Entity>()
                .SelectMany(e => e.Entity.DomainEvents)
                .ToList();

            foreach (var entry in context.ChangeTracker.Entries<Entity>())
                entry.Entity.ClearDomainEvents();

            foreach (var domainEvent in domainEvents)
                await _publisher.Publish(domainEvent, cancellationToken);

            return result;
        }
    }
}

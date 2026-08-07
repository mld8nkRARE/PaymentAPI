using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PaymentAPI.Primitives;

namespace PaymentAPI.Infrastructure.Configurations
{
    public class ProcessedDomainEventConfiguration : IEntityTypeConfiguration<ProcessedDomainEvent>
    {
        public void Configure(EntityTypeBuilder<ProcessedDomainEvent> builder)
        {
            builder.HasKey(e => e.EventId);
            builder.Property(e => e.EventId).HasConversion<DomainEventId.EfCoreValueConverter>()
                .ValueGeneratedNever();

            builder.Property(e => e.ProcessedAt)
                .HasDefaultValueSql("NOW()");
        }
    }
}
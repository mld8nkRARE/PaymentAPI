using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace PaymentAPI.Infrastructure.Configurations
{
    public class OutboxMessageConfiguration : IEntityTypeConfiguration<OutboxMessage>
    {
        public void Configure(EntityTypeBuilder<OutboxMessage> builder)
        {
            builder.HasKey(m => m.Id);

            builder.Property(m => m.Type)
                .HasMaxLength(255)
                .IsRequired();

            builder.Property(m => m.Content)
                .HasColumnType("text")
                .IsRequired();

            builder.Property(m => m.OccurredOn)
                .HasDefaultValueSql("NOW()");

            builder.Property(m => m.Error)
                .HasMaxLength(1000);

            builder.Property(m => m.Attempts)
                .HasDefaultValue(0);

            builder.HasIndex(m => new { m.ProcessedOn, m.OccurredOn });
        }
    }
}
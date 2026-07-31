using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PaymentAPI.Domain.Refunds;
using PaymentAPI.Primitives;

namespace PaymentAPI.Infrastructure.Configurations
{
    public class RefundConfiguration : IEntityTypeConfiguration<Refund>
    {
        public void Configure(EntityTypeBuilder<Refund> builder)
        {
            builder.HasKey(r => r.Id);
            builder.Property(r => r.Id).HasConversion<RefundId.EfCoreValueConverter>()
                .ValueGeneratedNever();
            builder.Property(r => r.PaymentId).HasConversion<PaymentId.EfCoreValueConverter>()
                .ValueGeneratedNever();
            builder.Property(r => r.OrderId).HasConversion<OrderId.EfCoreValueConverter>()
                .ValueGeneratedNever();
            builder.Property(r => r.ExternalRefundId).HasConversion<ExternalRefundId.EfCoreValueConverter>()
                .ValueGeneratedNever()
                .HasMaxLength(100);

            builder.HasIndex(r => r.ExternalRefundId)
                .IsUnique()
                .HasFilter("\"external_refund_id\" IS NOT NULL");
            builder.Property(r => r.Amount).HasPrecision(18, 2);
            builder.Property(r => r.Currency).HasMaxLength(3);
            builder.Property(r => r.Status).HasConversion<string>().HasMaxLength(50);
            builder.Property(r => r.CancellationParty).HasMaxLength(50);
            builder.Property(r => r.CancellationReason).HasMaxLength(100);
            builder.Property(r => r.Description).HasMaxLength(500);
            builder.Property(r => r.CreatedAt).HasDefaultValueSql("NOW()");

            builder.HasOne(r => r.Payment)
                .WithMany(p => p.Refunds)
                .HasForeignKey(r => r.PaymentId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}

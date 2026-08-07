using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PaymentAPI.Domain.Refunds;
using PaymentAPI.Primitives;

namespace PaymentAPI.Infrastructure.Configurations
{
    public class RefundItemConfiguration : IEntityTypeConfiguration<RefundItem>
    {
        public void Configure(EntityTypeBuilder<RefundItem> builder)
        {
            builder.HasKey(i => i.Id);
            builder.Property(i => i.Id).HasConversion<RefundItemId.EfCoreValueConverter>()
                .ValueGeneratedNever();
            builder.Property(i => i.RefundId).HasConversion<RefundId.EfCoreValueConverter>()
                .ValueGeneratedNever();
            builder.Property(i => i.ProductId).HasConversion<ProductId.EfCoreValueConverter>()
                .ValueGeneratedNever();

            builder.Property(i => i.Name).HasMaxLength(100).IsRequired();
            builder.Property(i => i.UnitPrice).HasPrecision(18, 2);
            builder.Property(i => i.TotalPrice).HasPrecision(18, 2);

            builder.HasOne(i => i.Refund)
                .WithMany(r => r.Items)
                .HasForeignKey(i => i.RefundId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(i => i.Product)
                .WithMany()
                .HasForeignKey(i => i.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Navigation(i => i.Refund).UsePropertyAccessMode(PropertyAccessMode.Field);
        }
    }
}
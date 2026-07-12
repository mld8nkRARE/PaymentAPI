using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PaymentAPI.Models;
using PaymentAPI.Primitives;

namespace PaymentAPI.Infrastructure.Configurations
{
    public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
    {
        public void Configure(EntityTypeBuilder<OrderItem> builder)
        {
            builder.HasKey(i => i.Id);
            builder.Property(i => i.Id).HasConversion<OrderItemId.EfCoreValueConverter>()
                .ValueGeneratedNever();

            builder.Property(i => i.Name).HasMaxLength(100).IsRequired();
            builder.Property(i => i.UnitPrice).HasPrecision(18, 2);
            builder.Property(i => i.TotalPrice).HasPrecision(18, 2);

            builder.HasOne(i => i.Order)
                .WithMany(o => o.OrderItems)
                .HasForeignKey(i => i.OrderId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(i => i.Product)
                .WithMany()
                .HasForeignKey(i=>i.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
            
        }
    }
}

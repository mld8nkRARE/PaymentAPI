using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PaymentAPI.Models;
using PaymentAPI.Primitives;

namespace PaymentAPI.Infrastructure.Configurations
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.HasKey(o => o.Id);
            builder.Property(o => o.Id).HasConversion<OrderId.EfCoreValueConverter>()
                .ValueGeneratedNever();

            builder.Property(o => o.CreatedAt).HasDefaultValueSql("NOW()");

            builder.Property(o => o.Status)
                .HasConversion<string>()
                .HasMaxLength(50);

            builder.Property(o => o.TotalPrice)
                .HasPrecision(18, 2);

            builder.HasOne(o => o.User)
                .WithMany(u => u.Orders)
                .HasForeignKey(o => o.UserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            builder.Navigation(o => o.OrderItems).UsePropertyAccessMode(PropertyAccessMode.Field);
        }
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PaymentAPI.Models;
using PaymentAPI.Primitives;
namespace PaymentAPI.Infrastructure.Configurations
{
    public class PaymentConfiguration:IEntityTypeConfiguration<Payment>
    {
        public void Configure(EntityTypeBuilder<Payment> builder)
        {
            builder.HasKey(p => p.Id);
            builder.Property(p => p.Id).HasConversion<PaymentId.EfCoreValueConverter>()
                .ValueGeneratedNever();
            builder.Property(p => p.ExternalPaymentId).HasConversion<ExternalPaymentId.EfCoreValueConverter>()
                .ValueGeneratedNever();

            builder.Property(p => p.Status)
                .HasConversion<string>()
                .HasMaxLength(50);

            builder.Property(p => p.Amount).HasPrecision(18, 2);
            builder.Property(p => p.Currency).HasMaxLength(3);
            builder.Property(p => p.Description).HasMaxLength(500);
            builder.Property(p => p.CreatedAt).HasDefaultValueSql("NOW()");

            builder.HasIndex(p => p.ExternalPaymentId)
                .IsUnique()
                .HasFilter("\"external_payment_id\" IS NOT NULL");

            builder.HasOne(p => p.Order)
                 .WithOne(o => o.Payment)
                 .HasForeignKey<Payment>(p => p.OrderId)
                 .IsRequired()
                 .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(p => p.User)
                .WithMany(u=>u.Payments)
                .HasForeignKey(p => p.UserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

        }   
    }
}

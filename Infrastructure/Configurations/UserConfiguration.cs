using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PaymentAPI.Models;
using PaymentAPI.Primitives;

namespace PaymentAPI.Infrastructure.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users");
            builder.HasKey(u => u.Id);
            builder.Property(u => u.Id).HasConversion<UserId.EfCoreValueConverter>()
                .ValueGeneratedNever();

            builder.Property(u => u.FullName).HasMaxLength(100);
            builder.Property(u => u.Email).HasMaxLength(255);
            builder.Property(u => u.PhoneNumber).HasMaxLength(15);
            builder.Property(u => u.PasswordHash).HasMaxLength(255);

            builder.HasIndex(u => u.Email).IsUnique();
            builder.HasIndex(u => u.PhoneNumber).IsUnique();

            builder.Navigation(u => u.Orders).UsePropertyAccessMode(PropertyAccessMode.Field);
            builder.Navigation(u => u.Payments).UsePropertyAccessMode(PropertyAccessMode.Field);

        }
    }
}

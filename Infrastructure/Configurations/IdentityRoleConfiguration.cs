using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PaymentAPI.Primitives;

namespace PaymentAPI.Infrastructure.Configurations
{
    public class IdentityRoleConfiguration : IEntityTypeConfiguration<IdentityRole<UserId>>
    {
        public void Configure(EntityTypeBuilder<IdentityRole<UserId>> builder)
        {
            builder.Property(r => r.Id)
                .HasConversion<UserId.EfCoreValueConverter>()
                .ValueGeneratedNever();
        }
    }
}

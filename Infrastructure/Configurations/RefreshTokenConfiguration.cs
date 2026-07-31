using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PaymentAPI.Domain;
using PaymentAPI.Primitives;

namespace PaymentAPI.Infrastructure.Configurations
{
    public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
    {
        public void Configure(EntityTypeBuilder<RefreshToken> builder)
        {
            builder.HasKey(t => t.Id);
            builder.Property(t => t.Id).HasConversion<RefreshTokenId.EfCoreValueConverter>()
                .ValueGeneratedNever();
            builder.Property(t => t.UserId).HasConversion<UserId.EfCoreValueConverter>()
                .ValueGeneratedNever();

            builder.Property(t => t.CreatedAt).HasDefaultValueSql("NOW()");
            builder.Property(t => t.Token).IsRequired().HasMaxLength(500);

            builder.HasIndex(t => t.Token).IsUnique();
            
            builder.HasOne(t => t.User)
                .WithMany(u => u.RefreshTokens)
                .HasForeignKey(t => t.UserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PaymentAPI.Models;
using PaymentAPI.Primitives;
namespace PaymentAPI.Infrastructure.Configurations
{ 
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.HasKey(p => p.Id);
            builder.Property(p => p.Id).HasConversion<ProductId.EfCoreValueConverter>()
                .ValueGeneratedNever();

            builder.Property(p => p.Name).HasMaxLength(50);
            builder.Property(p => p.Price).HasPrecision(18, 2);
            builder.Property(p => p.Description).HasMaxLength(500);
            builder.Property(p => p.IsDeleted).HasDefaultValue(false);

            builder.ToTable(t => t.HasCheckConstraint("CK_Products_StockQuantity_NonNegative",
            "\"stock_quantity\" >= 0"));
        }
    }
}

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PaymentAPI.Primitives;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using PaymentAPI.Domain.Payments;
using PaymentAPI.Domain;
using PaymentAPI.Domain.Refunds;

namespace PaymentAPI.Infrastructure
{
    public class ApplicationDbContext : IdentityDbContext<User, IdentityRole<UserId>, UserId>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
        public DbSet<Payment> Payments => Set<Payment>();
        public DbSet<Order> Orders => Set<Order>();
        public DbSet<OrderItem> OrderItems => Set<OrderItem>();
        public DbSet<Refund> Refunds => Set<Refund>();
        public DbSet<RefundItem> RefundItems => Set<RefundItem>();
        public DbSet<Product> Products => Set<Product>();
        public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
        public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();
        public DbSet<ProcessedDomainEvent> ProcessedDomainEvents => Set<ProcessedDomainEvent>();
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

            modelBuilder.Entity<IdentityRole<UserId>>(builder =>
            {
                builder.Property(r => r.Id)
                    .HasConversion<UserId.EfCoreValueConverter>()
                    .ValueGeneratedNever();
            });
        }
    }
}

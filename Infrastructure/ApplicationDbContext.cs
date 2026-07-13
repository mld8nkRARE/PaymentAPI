using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PaymentAPI.Models;
using PaymentAPI.Primitives;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace PaymentAPI.Infrastructure
{
    public class ApplicationDbContext : IdentityDbContext<User, IdentityRole<UserId>, UserId>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
        public DbSet<Payment> Payments => Set<Payment>();
        public DbSet<Order> Orders => Set<Order>();
        public DbSet<OrderItem> OrderItems => Set<OrderItem>();
        public DbSet<Product> Products => Set<Product>();
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        }
    }
}

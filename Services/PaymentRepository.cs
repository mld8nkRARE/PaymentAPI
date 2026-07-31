using Microsoft.EntityFrameworkCore;
using PaymentAPI.Infrastructure;
using PaymentAPI.Models;
using PaymentAPI.Primitives;

namespace PaymentAPI.Services
{
    public class PaymentRepository
    {
        private readonly ApplicationDbContext _db;
        public PaymentRepository(ApplicationDbContext db) => _db = db;
        public Task<Payment?> GetPaymentByExternalIdAsync(ExternalPaymentId externalId)
            => _db.Payments.Include(p => p.Order)
            .FirstOrDefaultAsync(p => p.ExternalPaymentId != null&& p.ExternalPaymentId == externalId);

    }
}

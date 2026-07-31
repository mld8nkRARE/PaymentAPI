using Microsoft.EntityFrameworkCore;
using PaymentAPI.Infrastructure;
using PaymentAPI.Models;
using PaymentAPI.Primitives;

namespace PaymentAPI.Services
{
    public class RefundRepository
    {
        private readonly ApplicationDbContext _db;
        public RefundRepository(ApplicationDbContext db) => _db = db;
        public Task<Refund?> GetRefundByExternalId(ExternalRefundId externalRefundId)
            => _db.Refunds.Include(r=>r.Payment).ThenInclude(p=>p.Order)
            .FirstOrDefaultAsync(r => r.ExternalRefundId == externalRefundId);
    }
}

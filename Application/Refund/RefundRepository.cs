using Microsoft.EntityFrameworkCore;
using PaymentAPI.Domain.Refund;
using PaymentAPI.Infrastructure;
using PaymentAPI.Primitives;

namespace PaymentAPI.Application.Refund
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

using Microsoft.EntityFrameworkCore;
using PaymentAPI.Domain.Primitives;
using PaymentAPI.Domain.Refunds;
using PaymentAPI.Infrastructure;
using PaymentAPI.Primitives;

namespace PaymentAPI.Application.Refunds
{
    public class RefundRepository
    {
        private readonly ApplicationDbContext _db;
        public RefundRepository(ApplicationDbContext db) => _db = db;
        public Task<Refund?> GetRefundByExternalId(ExternalRefundId externalRefundId)
            => _db.Refunds.Include(r=>r.Payment).ThenInclude(p=>p.Order)
            .FirstOrDefaultAsync(r => r.ExternalRefundId == externalRefundId);
        public Task<List<Refund>> GetPendingRefundsForPollingServiceAsync(int batchSize, CancellationToken ct) =>
        _db.Refunds
            .Include(r => r.Payment).ThenInclude(p => p.Order)
            .Include(r => r.Payment).ThenInclude(p => p.Refunds)
            .Where(r => r.Status == RefundStatus.Pending
                     && r.NextReconciliationCheckAt != null
                     && r.NextReconciliationCheckAt <= DateTime.UtcNow)
            .OrderBy(r => r.NextReconciliationCheckAt)
            .Take(batchSize)
            .ToListAsync(ct);
    }
}

using MediatR;
using Microsoft.EntityFrameworkCore;
using PaymentAPI.Domain.Refunds;
using PaymentAPI.Infrastructure;

namespace PaymentAPI.Application.EventHandlers
{
    public class ReturnProductToStockHandler : INotificationHandler<RefundSucceededEvent>
    {
        private readonly ApplicationDbContext _db;

        public ReturnProductToStockHandler(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task Handle(RefundSucceededEvent notification, CancellationToken ct)
        {
            if (await _db.ProcessedDomainEvents.AnyAsync(e => e.EventId == notification.EventId, ct))
                return;

            var refund = await _db.Refunds
                .Include(r => r.Items)
                .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(r => r.Id == notification.RefundId, ct)
                ?? throw new InvalidOperationException($"Возврат {notification.RefundId} не найден");

            foreach (var item in refund.Items)
            {
                item.Product.AddToStock(item.Quantity);
            }

            _db.ProcessedDomainEvents.Add(new ProcessedDomainEvent { EventId = notification.EventId });
            await _db.SaveChangesAsync(ct);
        }
    }
}
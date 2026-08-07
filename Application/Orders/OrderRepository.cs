using Microsoft.EntityFrameworkCore;
using PaymentAPI.Domain;
using PaymentAPI.Infrastructure;
using PaymentAPI.Primitives;

namespace PaymentAPI.Application.Orders
{
    public class OrderRepository
    {
        private readonly ApplicationDbContext _db;
        public OrderRepository(ApplicationDbContext db) => _db = db;

        public async Task<Order?> GetOrderWithItemsAsync(OrderId orderId, CancellationToken ct)
        {
            return await _db.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(o => o.Id == orderId, ct);
        }
    }
}

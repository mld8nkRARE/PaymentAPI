using PaymentAPI.DTO;
using PaymentAPI.Infrastructure;
using PaymentAPI.Models;
using PaymentAPI.Primitives;
using Microsoft.EntityFrameworkCore;

namespace PaymentAPI.Services
{
    public class OrderService
    {
        private readonly ApplicationDbContext _db;

        public OrderService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<OrderResponse> CreateOrderAsync(OrderCreateRequest request, UserId userId)
        {
            var order = new Order(userId);

            foreach (var item in request.Items)
            {
                var product = await _db.Products.FindAsync(item.ProductId)
                    ?? throw new ArgumentException($"Продукт {item.ProductId} не найден");

                if (product.IsDeleted)
                    throw new ArgumentException($"Продукт {item.ProductId} был удалён");

                order.AddItem(product, item.Quantity);
            }

            _db.Orders.Add(order);
            await _db.SaveChangesAsync();

            return FormResponse(order);
        }

        public async Task CancelOrderAsync(OrderId orderId, UserId userId)
        {
            var order = await _db.Orders
                .FirstOrDefaultAsync(o => o.Id == orderId && o.UserId == userId)
                ?? throw new InvalidOperationException($"Заказ {orderId} не найден");

            if (order.Status != OrderStatus.Pending)
                throw new InvalidOperationException($"Невозможно отменить заказ в статусе {order.Status}");

            order.ChangeOrderStatus(OrderStatus.Cancelled);
            await _db.SaveChangesAsync();
        }

        private static OrderResponse FormResponse(Order order)
        {
            var items = order.OrderItems.Select(i => new OrderItemResponse(
                i.Id,
                i.ProductId,
                i.Name,
                i.UnitPrice,
                i.Quantity,
                i.TotalPrice
            )).ToList();

            return new OrderResponse(
                order.Id,
                items,
                order.TotalPrice,
                order.Status.ToString(),
                order.CreatedAt
            );
        }
    }
}

using MediatR;
using PaymentAPI.Application.Orders;
using PaymentAPI.Domain.Payments;
using PaymentAPI.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace PaymentAPI.Application.EventHandlers
{
    public class CommitReservationHandler : INotificationHandler<PaymentSucceededEvent>
    {
        private readonly ApplicationDbContext _db;
        private readonly OrderRepository _orderRepository;

        public CommitReservationHandler(ApplicationDbContext db, OrderRepository orderRepository)
        {
            _db = db;
            _orderRepository = orderRepository;
        }

        public async Task Handle(PaymentSucceededEvent notification, CancellationToken ct)
        {
            if (await _db.ProcessedDomainEvents.AnyAsync(e => e.EventId == notification.EventId, ct))
                return;

            var order = await _orderRepository.GetOrderWithItemsAsync(notification.OrderId, ct)
                ?? throw new InvalidOperationException($"Заказ {notification.OrderId} не найден");

            foreach (var item in order.OrderItems)
            {
                item.Product.CommitReservation(item.Quantity);
            }

            _db.ProcessedDomainEvents.Add(new ProcessedDomainEvent { EventId = notification.EventId });
            await _db.SaveChangesAsync(ct);
        }
    }
}

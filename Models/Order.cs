using PaymentAPI.Primitives;
using System.ComponentModel.DataAnnotations;
namespace PaymentAPI.Models
{
    public class Order
    {
        public OrderId Id { get; private init; }
        public DateTime CreatedAt { get; private init; }
        public OrderStatus Status { get; private set; }
        public decimal TotalPrice { get; private set; }
        public UserId UserId { get; private init; }
        public User User { get; private set; } = null!;
        public Payment? Payment { get; private set; }
        [Timestamp]
        public uint Xmin { get; private set; }
        private readonly List<OrderItem> _orderItems = new();
        public IReadOnlyCollection<OrderItem> OrderItems => _orderItems.AsReadOnly();
        protected Order() { }
        public Order(UserId userId)
        {
            Id = OrderId.New();
            CreatedAt = DateTime.UtcNow;
            Status = OrderStatus.Pending;
            UserId = userId;
            TotalPrice = 0;
        }
        public void AddItem(Product product,int quantity = 1)
        {
            ArgumentNullException.ThrowIfNull(product);
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(quantity);

            var existingItem = _orderItems.FirstOrDefault(i => i.ProductId == product.Id);
            if (existingItem is not null)
            {
                existingItem.IncreaseQuantity(quantity);
            }
            else
            {
                var item = new OrderItem(this, product, quantity);
                _orderItems.Add(item);
            }
            RecalculateTotalPrice();
        }
        public void RemoveItem(OrderItemId orderItemId, int? quantityToRemove = null )
        {
            var item = _orderItems.FirstOrDefault(i => i.Id == orderItemId);
            if (item is null) return;

            if (quantityToRemove is not null)
            {
                if (item.Quantity <= quantityToRemove.Value)
                {
                    _orderItems.Remove(item);
                }
                else
                {
                    item.DecreaseQuantity(quantityToRemove.Value);
                }
            }
            else
            {
                _orderItems.Remove(item);
            }
            
            RecalculateTotalPrice();
        }
        private void RecalculateTotalPrice()
        {
            TotalPrice = _orderItems.Sum(i => i.TotalPrice);
        }
        public void ChangeStatus(OrderStatus status)
        {
            if (Status == status) return;
            Status = status;
        }   
    }
}

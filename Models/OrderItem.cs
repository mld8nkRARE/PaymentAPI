using PaymentAPI.Primitives;
namespace PaymentAPI.Models
{
    public class OrderItem
    {
        public OrderItemId Id { get; private init; }
        public  OrderId OrderId { get; private init; }
        public  ProductId ProductId { get; private init; }
        public string Name { get; private init; } = null!;
        public decimal TotalPrice { get; private set; }
        public decimal UnitPrice { get; private init; }
        public int Quantity { get; private set; }
        public Order Order { get; private set; } = null!;
        public Product Product { get; private set; } = null!;
        protected OrderItem() { }
        public OrderItem(Order order,Product product, int quantity)
        {
            ArgumentNullException.ThrowIfNull(order);
            ArgumentNullException.ThrowIfNull(product);
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(quantity, nameof(quantity));

            Id = OrderItemId.New();
            OrderId = order.Id;
            ProductId = product.Id;
            Name = product.Name;
            UnitPrice = product.Price;
            Quantity = quantity;
            RecalculateTotalPrice();
        }
        private void RecalculateTotalPrice()
        {
            TotalPrice = UnitPrice * Quantity;
        }
        public void IncreaseQuantity(int quantityToAdd)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(quantityToAdd, nameof(quantityToAdd));

            Quantity += quantityToAdd;
            RecalculateTotalPrice();
        }
        public void DecreaseQuantity(int quantityToRemove)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(quantityToRemove, nameof(quantityToRemove));
            ArgumentOutOfRangeException.ThrowIfGreaterThan(quantityToRemove,Quantity, nameof(quantityToRemove));

            Quantity -= quantityToRemove;
            RecalculateTotalPrice();
        }
        
    }
}

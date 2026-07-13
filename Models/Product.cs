using PaymentAPI.Primitives;
namespace PaymentAPI.Models
{
    public class Product
    {
        public ProductId Id { get; private init; }
        public string Name { get; private set; } = null!;
        public decimal Price { get; private set; }
        public string? Description { get; private set; }
        public int StockQuantity { get; private set; }
        protected Product() { }
        public Product(string name, decimal price, string? description = null, int stockQuantity = 0)
        {
            ArgumentNullException.ThrowIfNullOrEmpty(name, nameof(name));
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(price);
            ArgumentOutOfRangeException.ThrowIfNegative(stockQuantity);
            Id = ProductId.New();
            Name = name;
            Price = price;
            Description = description;
            StockQuantity = stockQuantity;
        }
        public void AddToStock(int quantityToAdd)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(quantityToAdd, nameof(quantityToAdd));
            StockQuantity += quantityToAdd;
        }
        public void RemoveFromStock(int quantityToRemove)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(quantityToRemove, nameof(quantityToRemove));
            if (StockQuantity < quantityToRemove)
                throw new InvalidOperationException("Недостаточно товаров на складе");

            StockQuantity -= quantityToRemove;
            
        }
        public void ChangePrice(decimal price)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(price, nameof(price));
            Price = price;
        }
        public void ChangeDescription(string description)
        {
            Description = description;
        }
        public void ChangeName(string name)
        {
            ArgumentNullException.ThrowIfNullOrEmpty(name, nameof(name));
            if (Name == name)
                return;
            Name = name;
        }
    }
}

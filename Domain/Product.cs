using PaymentAPI.Primitives;
namespace PaymentAPI.Domain
{
    public class Product : Entity
    {
        public ProductId Id { get; private init; }
        public string Name { get; private set; } = null!;
        public decimal Price { get; private set; }
        public string? Description { get; private set; }
        public bool IsDeleted { get; private set; }
        public int StockQuantity { get; private set; }
        public int ReservedQuantity { get; private set; }

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
            ReservedQuantity = 0;
        }
        
        public void ReserveStock(int quantityToReserve)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(quantityToReserve, nameof(quantityToReserve));
            if (StockQuantity - ReservedQuantity < quantityToReserve)
                throw new InvalidOperationException("Недостаточно свободных товаров на складе для резервирования");

            ReservedQuantity += quantityToReserve;
        }

        public void CancelReservation(int quantityToCancel)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(quantityToCancel, nameof(quantityToCancel));
            if (ReservedQuantity < quantityToCancel)
                throw new InvalidOperationException("Недостаточно зарезервированных товаров");

            ReservedQuantity -= quantityToCancel;
        }

        public void CommitReservation(int quantityToCommit)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(quantityToCommit, nameof(quantityToCommit));
            if (ReservedQuantity < quantityToCommit)
                throw new InvalidOperationException("Недостаточно зарезервированных товаров для списания");

            ReservedQuantity -= quantityToCommit;
            StockQuantity -= quantityToCommit;
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
        public void MarkAsDeleted()
        {
            IsDeleted = true;
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

using PaymentAPI.Primitives;

namespace PaymentAPI.Domain.Refunds
{
    public class RefundItem
    {
        public RefundItemId Id { get; private init; }
        public RefundId RefundId { get; private init; }
        public ProductId ProductId { get; private init; }
        public string Name { get; private init; } = null!;
        public int Quantity { get; private init; }
        public decimal UnitPrice { get; private init; }
        public decimal TotalPrice { get; private init; }

        public Refund Refund { get; private set; } = null!;
        public Product Product { get; private set; } = null!;

        protected RefundItem() { }

        public RefundItem(Refund refund, Product product, int quantity, decimal unitPrice)
        {
            ArgumentNullException.ThrowIfNull(refund);
            ArgumentNullException.ThrowIfNull(product);
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(quantity, nameof(quantity));
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(unitPrice, nameof(unitPrice));

            Id = RefundItemId.New();
            RefundId = refund.Id;
            ProductId = product.Id;
            Name = product.Name;
            Quantity = quantity;
            UnitPrice = unitPrice;
            TotalPrice = unitPrice * quantity;
        }
    }
}
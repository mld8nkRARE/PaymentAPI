using PaymentAPI.Primitives;

namespace PaymentAPI.DTO
{
    public record CreateOrderRequest(List<OrderItemRequest> Items);
    public record OrderItemRequest(ProductId ProductId, int Quantity);
}

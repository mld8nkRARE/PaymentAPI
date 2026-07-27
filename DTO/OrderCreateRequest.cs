using PaymentAPI.Primitives;

namespace PaymentAPI.DTO
{
    public record OrderCreateRequest(List<OrderItemRequest> Items);
    public record OrderItemRequest(ProductId ProductId, int Quantity);
}
using PaymentAPI.Primitives;

namespace PaymentAPI.DTO.order
{
    public record OrderCreateRequest(List<OrderItemRequest> Items);
    public record OrderItemRequest(ProductId ProductId, int Quantity);
}
using PaymentAPI.Primitives;

namespace PaymentAPI.DTO
{
    public record OrderResponse(
        OrderId Id,
        List<OrderItemResponse> Items,
        decimal TotalPrice,
        string Status,
        DateTime CreatedAt);

    public record OrderItemResponse(
        OrderItemId Id,
        ProductId ProductId,
        string Name,
        decimal UnitPrice,
        int Quantity,
        decimal TotalPrice);
}

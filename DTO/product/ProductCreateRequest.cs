namespace PaymentAPI.DTO.product
{
    public record ProductCreateRequest(string Name, decimal Price, string? Description = null, int StockQuantity = 0);
}
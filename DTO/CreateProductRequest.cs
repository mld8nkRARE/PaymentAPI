namespace PaymentAPI.DTO
{
    public record CreateProductRequest(string Name, decimal Price, string? Description = null, int StockQuantity = 0);
}

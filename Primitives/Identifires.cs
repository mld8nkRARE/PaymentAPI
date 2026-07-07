namespace PaymentAPI.Primitives
{
    public readonly record struct UserId(Guid id);
    public readonly record struct PaymentId(Guid id);
    public readonly record struct ExternalPaymentId(Guid id);
    public readonly record struct OrderId(Guid id);
    public readonly record struct ProductId(Guid id);
    public readonly record struct OrderItemId(Guid id);
}

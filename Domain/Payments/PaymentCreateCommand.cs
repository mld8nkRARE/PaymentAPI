using PaymentAPI.Primitives;

namespace PaymentAPI.Domain.Payments
{
    public abstract record PaymentCreateCommand
    (
        decimal Amount,
        string Currency,
        OrderId OrderId,
        string? Description
    )
    {
       abstract public string ProviderName { get; }
    };
    
}

using PaymentAPI.Primitives;

namespace PaymentAPI.Domain.Payment
{
    public abstract record PaymentCreateCommand
    (
        decimal Amount,
        string Currency,
        OrderId OrderId,
        string? Description
    );
    
}

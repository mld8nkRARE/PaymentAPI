using PaymentAPI.Primitives;

namespace PaymentAPI.DTO.payment
{
    public abstract record PaymentCreateCommand
    (
        decimal Amount,
        string Currency,
        OrderId OrderId,
        string? Description
    );
    
}

using PaymentAPI.Primitives;

namespace PaymentAPI.DTO.payment
{
    public record PaymentCreateYookassaCommand(
        decimal Amount,
        string Currency,
        OrderId OrderId,
        string? Description) : PaymentCreateCommand(Amount, Currency,OrderId,Description);
}

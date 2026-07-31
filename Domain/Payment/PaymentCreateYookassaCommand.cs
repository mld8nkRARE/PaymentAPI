using PaymentAPI.Primitives;

namespace PaymentAPI.Domain.Payment
{
    public record PaymentCreateYookassaCommand(
        decimal Amount,
        string Currency,
        OrderId OrderId,
        string? Description) : PaymentCreateCommand(Amount, Currency,OrderId,Description);
}

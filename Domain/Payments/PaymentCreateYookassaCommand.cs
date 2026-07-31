using PaymentAPI.Primitives;

namespace PaymentAPI.Domain.Payments
{
    public record PaymentCreateYookassaCommand(
        decimal Amount,
        string Currency,
        OrderId OrderId,
        string? Description) : PaymentCreateCommand(Amount, Currency,OrderId,Description);
}

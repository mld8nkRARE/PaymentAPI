using PaymentAPI.Primitives;

namespace PaymentAPI.Domain.Refund
{
    public record RefundCreateYookassaCommand
    (
        ExternalPaymentId PaymentId,
        decimal Amount,
        string Currency,
        string? Description
    ) : RefundCreateCommand(PaymentId, Amount, Currency, Description);
}

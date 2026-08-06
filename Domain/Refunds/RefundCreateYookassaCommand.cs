using PaymentAPI.Primitives;

namespace PaymentAPI.Domain.Refunds
{
    public record RefundCreateYookassaCommand
    (
        ExternalPaymentId PaymentId,
        decimal Amount,
        string Currency,
        string? Description
    ) : RefundCreateCommand(PaymentId, Amount, Currency, Description)
    {
        public override string ProviderName => "yookassa";
    }
}

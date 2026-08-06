using PaymentAPI.Primitives;

namespace PaymentAPI.Domain.Refunds
{
    public abstract record RefundCreateCommand
    (
        ExternalPaymentId PaymentId,
        decimal Amount,
        string Currency,
        string? Description
    )
    {
        public abstract string ProviderName { get; }
    }
}

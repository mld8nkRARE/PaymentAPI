using PaymentAPI.Primitives;

namespace PaymentAPI.Domain.Refund
{
    public abstract record RefundCreateCommand
    (
        ExternalPaymentId PaymentId,
        decimal Amount,
        string Currency,
        string? Description
    );
}

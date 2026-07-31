using PaymentAPI.Primitives;

namespace PaymentAPI.DTO.refund
{
    public abstract record RefundCreateCommand
    (
        ExternalPaymentId PaymentId,
        decimal Amount,
        string Currency,
        string? Description
    );
}

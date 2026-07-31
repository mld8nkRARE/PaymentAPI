using PaymentAPI.Primitives;

namespace PaymentAPI.Domain.Refunds
{
    public record RefundSucceededEvent(
        RefundId RefundId,
        PaymentId PaymentId,
        OrderId OrderId,
        decimal RefundedAmount,
        bool IsFullRefund) : IDomainEvent;
}

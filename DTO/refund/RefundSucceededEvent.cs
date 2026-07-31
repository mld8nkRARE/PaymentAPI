using PaymentAPI.Interfaces;
using PaymentAPI.Primitives;

namespace PaymentAPI.DTO.refund
{
    public record RefundSucceededEvent(
        RefundId RefundId,
        PaymentId PaymentId,
        OrderId OrderId,
        decimal RefundedAmount,
        bool IsFullRefund) : IDomainEvent;
}

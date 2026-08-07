using MediatR;
using PaymentAPI.Primitives;
using PaymentAPI.Domain.Refunds;

namespace PaymentAPI.Domain.Refunds
{
    public record RefundSucceededEvent(
        DomainEventId EventId,
        RefundId RefundId,
        PaymentId PaymentId,
        OrderId OrderId,
        decimal RefundedAmount,
        bool IsFullRefund) : IDomainEvent;
}

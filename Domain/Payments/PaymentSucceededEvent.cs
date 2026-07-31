using PaymentAPI.Primitives;

namespace PaymentAPI.Domain.Payments
{
    public record PaymentSucceededEvent(
    PaymentId PaymentId,
    OrderId OrderId,
    decimal Amount,
    string Currency) : IDomainEvent;
}

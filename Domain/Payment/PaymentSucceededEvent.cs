using PaymentAPI.Primitives;

namespace PaymentAPI.Domain.Payment
{
    public record PaymentSucceededEvent(
    PaymentId PaymentId,
    OrderId OrderId,
    decimal Amount,
    string Currency) : IDomainEvent;
}

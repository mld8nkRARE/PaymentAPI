using PaymentAPI.Interfaces;
using PaymentAPI.Primitives;

namespace PaymentAPI.DTO.events
{
    public record PaymentSucceededEvent(
    PaymentId PaymentId,
    OrderId OrderId,
    decimal Amount,
    string Currency) : IDomainEvent;
}

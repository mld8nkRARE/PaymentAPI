using PaymentAPI.Domain.Payments;

namespace PaymentAPI.DTO.payment
{
    public record PaymentCreateYookassaRequest : PaymentCreateRequest
    {
        override public PaymentCreateYookassaCommand ToCommand(decimal amount)
            => new PaymentCreateYookassaCommand(amount, Currency, OrderId, Description);
    }
}

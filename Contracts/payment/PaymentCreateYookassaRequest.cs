using PaymentAPI.Domain.Payments;

namespace PaymentAPI.DTO.payment
{
    public record PaymentCreateYookassaRequest : PaymentCreateRequest
    {
        override public PaymentCreateYookassaCommand ToCommand()
            => new PaymentCreateYookassaCommand(Amount,Currency,OrderId,Description);
    }
}

using PaymentAPI.Domain.Refund;
using PaymentAPI.Primitives;

namespace PaymentAPI.DTO.refund
{
    public record RefundCreateYookassaRequest : RefundCreateRequest
    {
        override public RefundCreateCommand ToCommand()
            => new RefundCreateYookassaCommand(ExternalPaymentId,Amount,Currency,Description);
    }
}

using PaymentAPI.Domain.Refunds;
using PaymentAPI.Primitives;

namespace PaymentAPI.DTO.refund
{
    public record RefundCreateYookassaRequest : RefundCreateRequest
    {
        override public RefundCreateCommand ToCommand()
            => new RefundCreateYookassaCommand(ExternalPaymentId,Amount,Currency,Description);
    }
}

using PaymentAPI.Domain.Refunds;
using PaymentAPI.DTO.refund;

namespace PaymentAPI.Providers.Interfaces
{
    public interface IRefundGateway
    {
        string ProviderName { get; }
        Task<RefundResult> CreateRefundAsync(RefundCreateCommand cmd, string idempotenceKey);
    }

    public interface IRefundGateway<in TCommand> : IRefundGateway where TCommand : RefundCreateCommand
    {
        Task<RefundResult> CreateRefundAsync(TCommand cmd, string idempotenceKey);
    }
}

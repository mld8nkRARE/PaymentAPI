using PaymentAPI.DTO.refund;
using System.Text.Json;

namespace PaymentAPI.Interfaces
{
    public interface IRefundGateway { }
    public interface IRefundGateway<in TCommand> where TCommand:RefundCreateCommand
    {
        Task<RefundResult> CreateRefundAsync(TCommand cmd, string idempotenceKey);
        Task<RefundResult> GetRefundAsync(string refundId);
    }
}

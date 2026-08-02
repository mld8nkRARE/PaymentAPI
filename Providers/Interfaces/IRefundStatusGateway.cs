using PaymentAPI.DTO.refund;
using PaymentAPI.Primitives;

namespace PaymentAPI.Providers.Interfaces
{
    public interface IRefundStatusGateway
    {
        string ProviderName { get; }
        Task<RefundResult> GetRefundAsync(ExternalRefundId refundId);
    }
}

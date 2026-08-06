using PaymentAPI.Domain.Payments;
using PaymentAPI.DTO.payment;

namespace PaymentAPI.Providers.Interfaces
{
    public interface IPaymentGateway
    {
        string ProviderName { get; }
        Task<PaymentResult> CreatePaymentAsync(PaymentCreateCommand cmd, string idempotenceKey);
    }

    public interface IPaymentGateway<in TCommand> : IPaymentGateway where TCommand : PaymentCreateCommand
    {
        Task<PaymentResult> CreatePaymentAsync(TCommand cmd, string idempotenceKey);
    }
}

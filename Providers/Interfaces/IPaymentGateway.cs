using PaymentAPI.Domain.Payment;
using PaymentAPI.DTO.payment;
using PaymentAPI.DTO.refund;
using System.Text.Json;

namespace PaymentAPI.Providers.Interfaces
{
    public interface IPaymentGateway { }
    public interface IPaymentGateway<in TCommand> where TCommand : PaymentCreateCommand
    {
        Task<PaymentResult> CreatePaymentAsync(TCommand cmd, string idempotenceKey);
    }
}

using PaymentAPI.DTO;
namespace PaymentAPI.Interfaces
{
    public interface IPaymentGateway
    {
        Task<PaymentResult> CreatePayment(PaymentRequest request, string idempotenceKey);
    }
}

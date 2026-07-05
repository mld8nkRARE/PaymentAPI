using PaymentAPI.Primitives;
using PaymentAPI.DTO;
using Yandex.Checkout.V3;
namespace PaymentAPI.Interfaces
{
    public interface IPaymentGateway
    {
        Task<PaymentResult> CreatePayment(PaymentRequest request, string idempotenceKey);
       
    }
}

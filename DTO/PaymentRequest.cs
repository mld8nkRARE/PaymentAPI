using Yandex.Checkout.V3;
namespace PaymentAPI.DTO
{
    public class PaymentRequest
    {
        public Amount Amount { get; set; } = new();
        public string Description { get; set; } = string.Empty;

    }
}

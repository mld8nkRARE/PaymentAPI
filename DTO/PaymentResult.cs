using Yandex.Checkout.V3;
using PaymentAPI.Primitives;
namespace PaymentAPI.DTO
{
    public class PaymentResult
    {
        public PaymentStatus Status { get; set; }
        public PaymentId PaymentId { get; set; }
        public required Amount Amount { get; init; }
        public required string ConfirmationUrl { get; init; }
        public DateTime CreatedAt { get; init; }
    }
}

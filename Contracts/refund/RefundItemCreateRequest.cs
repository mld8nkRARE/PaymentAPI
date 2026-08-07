using PaymentAPI.Primitives;
using System.ComponentModel.DataAnnotations;

namespace PaymentAPI.DTO.refund
{
    public record RefundItemCreateRequest(
        [Required] ProductId ProductId,
        [Required, Range(1, int.MaxValue)] int Quantity);
}
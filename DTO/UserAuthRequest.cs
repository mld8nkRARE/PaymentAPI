using System.ComponentModel.DataAnnotations;

namespace PaymentAPI.DTO
{
    public record UserAuthRequest([Required][EmailAddress] string Email, [Required] string Password);
}

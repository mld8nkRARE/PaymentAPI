using System.ComponentModel.DataAnnotations;

namespace PaymentAPI.DTO
{
    public record AuthUserRequest([Required][EmailAddress] string Email, [Required] string Password);
}
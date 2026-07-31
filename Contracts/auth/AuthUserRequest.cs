using System.ComponentModel.DataAnnotations;

namespace PaymentAPI.DTO.auth
{
    public record AuthUserRequest([Required][EmailAddress] string Email, [Required] string Password);
}
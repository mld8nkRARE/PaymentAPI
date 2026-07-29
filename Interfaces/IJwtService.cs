using PaymentAPI.DTO.auth;
using PaymentAPI.Models;
namespace PaymentAPI.Interfaces
{
    public interface IJwtService
    {
        AuthUserResponse GenerateTokens(User user);
    }
}

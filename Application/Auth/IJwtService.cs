using PaymentAPI.Domain;
using PaymentAPI.DTO.auth;
namespace PaymentAPI.Application.Auth
{
    public interface IJwtService
    {
        AuthUserResponse GenerateTokens(User user);
    }
}

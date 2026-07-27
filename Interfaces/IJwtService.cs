using PaymentAPI.DTO;
using PaymentAPI.Models;
namespace PaymentAPI.Interfaces
{
    public interface IJwtService
    {
        AuthUserResponse GenerateTokens(User user);
    }
}

using PaymentAPI.DTO;
using PaymentAPI.Models;
namespace PaymentAPI.Interfaces
{
    public interface IJwtService
    {
        UserAuthResponse GenerateTokens(User user);
    }
}

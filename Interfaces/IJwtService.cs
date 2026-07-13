using PaymentAPI.Models;
namespace PaymentAPI.Interfaces
{
    public interface IJwtService
    {
        public string GenerateToken(User user);
       // public Task VerifyToken(User user);
        //public Task JwtDecoder();
    }
}

using Microsoft.IdentityModel.Tokens;
using PaymentAPI.Interfaces;
using PaymentAPI.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PaymentAPI.Services
{
    public class JwtService : IJwtService
    {
        private readonly IConfiguration _configuration;
        public JwtService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GenerateToken(User user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:key"]));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var claims = new List<Claim>
            {
                new Claim("sub", user.Id.ToString()),
                new Claim("email", user.Email),
                new Claim("role", "User")
            };
            var token =  new JwtSecurityToken(
                issuer: _configuration["Jwt:iss"],
                audience: _configuration["Jwt:aud"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(int.Parse(_configuration["Jwt:exp"])),
                signingCredentials: credentials
            );
            ArgumentNullException.ThrowIfNull(token, nameof(token));
            return token.ToString();
        }
        
    }
}

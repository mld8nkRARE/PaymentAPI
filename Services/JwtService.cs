using Microsoft.IdentityModel.Tokens;
using PaymentAPI.DTO.auth;
using PaymentAPI.Interfaces;
using PaymentAPI.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace PaymentAPI.Services
{
    public class JwtService : IJwtService
    {
        private readonly IConfiguration _configuration;
        public JwtService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public AuthUserResponse GenerateTokens(User user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:key"]));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim("sub", user.Id.ToString()),
                new Claim("email", user.Email),
                new Claim("role", "User")
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:iss"],
                audience: _configuration["Jwt:aud"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(int.Parse(_configuration["Jwt:exp"])),
                signingCredentials: credentials
            );

            var handler = new JwtSecurityTokenHandler();
            var accessToken = handler.WriteToken(token);
            var refreshToken = GenerateRefreshToken();
            var accessTokenExpireIn = (int) (token.ValidTo - DateTime.UtcNow).TotalSeconds ;
            var refreshTokenExpireIn = GetUserRefreshTokenExpireInSeconds();

            return new AuthUserResponse(accessToken, "Bearer", refreshToken, accessTokenExpireIn,refreshTokenExpireIn);
        }
        private string GenerateRefreshToken() 
        {
            return RandomNumberGenerator.GetHexString(16);
        }
        private int GetUserRefreshTokenExpireInSeconds()
        {
            return (int)(TimeSpan.FromDays(int.Parse(_configuration["RefreshTokenExpireAt"]))).TotalSeconds;
        }
    }
}

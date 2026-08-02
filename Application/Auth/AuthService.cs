using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PaymentAPI.Domain;
using PaymentAPI.DTO.auth;
using PaymentAPI.Infrastructure;
using PaymentAPI.Primitives;
using System.Data;
using System.Security.Cryptography;
using System.Text;

namespace PaymentAPI.Application.Auth
{
    public class AuthService : IAuthService
    {
        private readonly IJwtService _jwtService;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly RefreshTokenRepository _refreshTokenRepository;
        private readonly ApplicationDbContext _db;

        public AuthService(UserManager<User> userManager, SignInManager<User> signInManager,
            IJwtService jwtService, RefreshTokenRepository refreshTokenRepository, ApplicationDbContext db)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtService = jwtService;
            _refreshTokenRepository = refreshTokenRepository;
            _db = db;
        }

        public async Task<AuthUserResponse> RegisterAsync(AuthUserRequest request)
        {
            var user = new User(request.Email);
            var result = await _userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded)
                throw new InvalidDataException(string.Join("\n", result.Errors.Select(er => er.Description)));

            return await IssueTokensAsync(user);
        }

        public async Task<AuthUserResponse> LoginAsync(AuthUserRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user is null)
                throw new InvalidCredentialsException();
            
            var result = await _signInManager.CheckPasswordSignInAsync(user,request.Password,true);
            if (!result.Succeeded)
                throw new InvalidCredentialsException();

            return await IssueTokensAsync(user);
        }

        public async Task<AuthUserResponse> RefreshTokenAsync(AuthRefreshRequest authRefreshRequest)
        {
            var refreshTokenHash = Hash(authRefreshRequest.RefreshToken);
            var oldToken = await _refreshTokenRepository.GetRefreshTokenByHashAsync(refreshTokenHash)
                ?? throw new RefreshTokenNotFoundException();

            var user = await _userManager.FindByIdAsync(oldToken.UserId.ToString())
                ?? throw new InvalidDataException("Пользователь не найден");

            if (oldToken.IsRevoked)
            {
                await LogoutAllAsync(user.Id);
                await _db.SaveChangesAsync();
                throw new RefreshTokenReusedException();
            }

            if (oldToken.IsExpire)
            {
                throw new RefreshTokenExpiredException();
            }

            var userAuthResponse = _jwtService.GenerateTokens(user);
            var newToken = CreateRefreshToken(userAuthResponse.RefreshToken, userAuthResponse.RefreshTokenExpiresIn, user.Id);

            oldToken.RotateTo(newToken.Id);
            _refreshTokenRepository.Add(newToken);
            
            try
            {
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                await LogoutAllAsync(oldToken.UserId);
                await _db.SaveChangesAsync();
                throw new RefreshTokenReusedException();
            }
            return userAuthResponse;
        }

        private async Task<AuthUserResponse> IssueTokensAsync(User user)
        {
            var jwtResult = _jwtService.GenerateTokens(user);
            var refreshToken = CreateRefreshToken(jwtResult.RefreshToken, jwtResult.RefreshTokenExpiresIn, user.Id);

            _refreshTokenRepository.Add(refreshToken);
            await _db.SaveChangesAsync();

            return jwtResult;
        }

        private static RefreshToken CreateRefreshToken(string token, int refreshTokenExpiresIn, UserId userId)
        {
            var tokenHash = Hash(token);
            var refreshTokenExpireAt = DateTime.UtcNow.AddSeconds(refreshTokenExpiresIn);

            return new RefreshToken(tokenHash, userId, refreshTokenExpireAt);
        }
        public async Task LogoutAsync(string refreshToken)
        {
            var tokenHash = Hash(refreshToken);
            var token = await _refreshTokenRepository.GetRefreshTokenByHashAsync(tokenHash);
            if (token is null) return;

            token.Revoke();
            await _db.SaveChangesAsync();
        }

        public async Task LogoutAllAsync(UserId userId)
        {
            var allUserTokens = await _refreshTokenRepository.GetActiveRefreshTokensByUserIdAsync(userId);
            foreach (var t in allUserTokens) t.Revoke();
            await _db.SaveChangesAsync();
        }
        public static string Hash(string token) =>
           Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(token)));
    }
}

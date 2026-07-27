using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PaymentAPI.DTO;
using PaymentAPI.Infrastructure;
using PaymentAPI.Interfaces;
using PaymentAPI.Models;
using PaymentAPI.Primitives;

namespace PaymentAPI.Services
{
    public class AuthService : IAuthService
    {
        private readonly IJwtService _jwtService;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ApplicationDbContext _db;
        public AuthService(UserManager<User> userManager, SignInManager<User> signInManager,
            IJwtService jwtService, ApplicationDbContext db)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtService = jwtService;
            _db = db;
        }
        public async Task<AuthUserResponse> RegisterAsync(AuthUserRequest request)
        {
            var user = new User(request.Email);
            var result = await _userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded)
                throw new InvalidDataException(string.Join("\n", result.Errors.Select(er => er.Description)));

            var userAuthResponse = _jwtService.GenerateTokens(user);
            await SaveRefreshTokenAsync(userAuthResponse.RefreshToken,
               userAuthResponse.RefreshTokenExpiresIn, user.Id);

            return userAuthResponse;
        }
        public async Task<AuthUserResponse> LoginAsync(AuthUserRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user is null)
            {
                throw new InvalidDataException("Неверная почта или пароль");
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user,request.Password,true);
            if (!result.Succeeded)
            {
                throw new InvalidDataException("Неверная почта или пароль");
            }

            var userAuthResponse = _jwtService.GenerateTokens(user);
            await SaveRefreshTokenAsync(userAuthResponse.RefreshToken,
                userAuthResponse.RefreshTokenExpiresIn, user.Id);

            return userAuthResponse;
        }
        public async Task<AuthUserResponse> RefreshTokenAsync(AuthRefreshRequest refreshToken)
        {
            var oldToken = await _db.RefreshTokens.FirstOrDefaultAsync(t => t.Token == refreshToken.oldToken)
                ?? throw new InvalidDataException("Токен не найден");

            if (!oldToken.IsActive)
            {
                await RevokeAll(oldToken);
                await _db.SaveChangesAsync();
                throw new UnauthorizedAccessException("Токен отозван");
            }

            if (oldToken.ExpireAt <= DateTime.UtcNow)
            {
                oldToken.Revoke();
                await _db.SaveChangesAsync();
                throw new UnauthorizedAccessException("Токен истёк");
            }

            var user = await _userManager.FindByIdAsync(oldToken.UserId.ToString())
                ?? throw new InvalidDataException("Пользователь не найден");

            var userAuthResponse = _jwtService.GenerateTokens(user);
            await SaveRefreshTokenAsync(userAuthResponse.RefreshToken,
                userAuthResponse.RefreshTokenExpiresIn, oldToken.UserId);

            oldToken.Revoke();
            await _db.SaveChangesAsync();

            return userAuthResponse;
        }
        private async Task SaveRefreshTokenAsync(string token, int refreshTokenExpiresIn, UserId userId)
        {
            var refreshTokenExpireAt = DateTime.UtcNow.AddSeconds(refreshTokenExpiresIn);
            RefreshToken refreshToken = new RefreshToken(token, userId, refreshTokenExpireAt);
            await _db.RefreshTokens.AddAsync(refreshToken);
            await _db.SaveChangesAsync();
        }

        private async Task RevokeAll(RefreshToken oldToken)
        {
            var allUserTokens = await _db.RefreshTokens
                    .Where(t => t.UserId == oldToken.UserId && t.IsActive)
                    .ToListAsync();
            foreach (var t in allUserTokens) t.Revoke();
        }
    }
}

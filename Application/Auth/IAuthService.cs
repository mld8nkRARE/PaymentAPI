using PaymentAPI.DTO.auth;
using PaymentAPI.Primitives;

namespace PaymentAPI.Application.Auth
{
    public interface IAuthService
    {
        Task<AuthUserResponse> RegisterAsync(AuthUserRequest userAuthRequest);
        Task<AuthUserResponse> LoginAsync(AuthUserRequest userAuthRequest);
        Task<AuthUserResponse> RefreshTokenAsync(AuthRefreshRequest userRefreshAuthTokenRequest);
        Task LogoutAsync(string refreshToken);
        Task LogoutAllAsync(UserId userId);
    }
}

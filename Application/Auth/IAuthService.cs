using PaymentAPI.DTO.auth;

namespace PaymentAPI.Application.Auth
{
    public interface IAuthService
    {
        Task<AuthUserResponse> RegisterAsync(AuthUserRequest userAuthRequest);
        Task<AuthUserResponse> LoginAsync(AuthUserRequest userAuthRequest);
        Task<AuthUserResponse> RefreshTokenAsync(AuthRefreshRequest userRefreshAuthTokenRequest);
    }
}

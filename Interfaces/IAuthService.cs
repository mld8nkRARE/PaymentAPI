using PaymentAPI.DTO;

namespace PaymentAPI.Interfaces
{
    public interface IAuthService
    {
        Task<UserAuthResponse> RegisterAsync(UserAuthRequest userAuthRequest);
        Task<UserAuthResponse> LoginAsync(UserAuthRequest userAuthRequest);
        Task<UserAuthResponse> RefreshTokenAsync(UserRefreshAuthTokenRequest userRefreshAuthTokenRequest);
    }
}

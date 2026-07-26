namespace PaymentAPI.DTO
{
    public record UserAuthResponse
    (
        string AccessToken,
        string TokenType,
        string RefreshToken,
        int AccessTokenExpiresIn,
        int RefreshTokenExpiresIn
    );
}

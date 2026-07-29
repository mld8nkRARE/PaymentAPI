namespace PaymentAPI.DTO.auth
{
    public record AuthUserResponse
    (
        string AccessToken,
        string TokenType,
        string RefreshToken,
        int AccessTokenExpiresIn,
        int RefreshTokenExpiresIn
    );
}
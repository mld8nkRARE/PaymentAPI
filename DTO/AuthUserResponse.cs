namespace PaymentAPI.DTO
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
namespace PaymentAPI.Application.Auth
{
    public class InvalidCredentialsException : Exception
    {
        public InvalidCredentialsException() : base("Неверная почта или пароль") { }
    }

    public class RefreshTokenNotFoundException : Exception
    {
        public RefreshTokenNotFoundException() : base("Токен не найден") { }
    }

    public class RefreshTokenExpiredException : Exception
    {
        public RefreshTokenExpiredException() : base("Токен истёк") { }
    }

    public class RefreshTokenReusedException : Exception
    {
        public RefreshTokenReusedException()
            : base("Обнаружено повторное использование токена — все сессии отозваны") { }
    }
}

namespace PaymentAPI.Interfaces
{
    public interface ISourceIpVerifier
    {
        string ProviderName { get; }
        bool VerifySourceIp(HttpContext httpContext);
    }
}

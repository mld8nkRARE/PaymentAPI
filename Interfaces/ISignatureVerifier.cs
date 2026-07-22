namespace PaymentAPI.Interfaces
{
    public interface ISignatureVerifier
    {
        string ProviderName {  get; }
        Task<bool> VerifySignatureAsync(HttpContext httpContext, string rawBody);
    }
}

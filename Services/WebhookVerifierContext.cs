using PaymentAPI.Interfaces;

namespace PaymentAPI.Services
{
    public class WebhookVerifierContext
    {
        private Dictionary<string, ISourceIpVerifier> _sourceIpVerifiers;
        private Dictionary<string, ISignatureVerifier> _signatureVerifiers;
        public WebhookVerifierContext(IEnumerable<ISourceIpVerifier> sourceIpVerifiers,
            IEnumerable<ISignatureVerifier> signatureVerifiers)
        {
            _sourceIpVerifiers = sourceIpVerifiers.ToDictionary(k => k.ProviderName,StringComparer.OrdinalIgnoreCase);
            _signatureVerifiers = signatureVerifiers.ToDictionary(k => k.ProviderName,StringComparer.OrdinalIgnoreCase);
        }
        public async Task<bool> VerifyAsync(string provider, HttpContext httpContext)
        {
            if (!_sourceIpVerifiers.TryGetValue(provider, out var sourceIpVerifier)
                || !_signatureVerifiers.TryGetValue(provider, out var signatureVerifier))
            {
                throw new ArgumentException("Провайдер не поддерживается", nameof(provider));
            }
            if (!sourceIpVerifier.VerifySourceIp(httpContext))
            {
                return false;
            }
            return await signatureVerifier.VerifySignatureAsync(httpContext);
        }
    }
}

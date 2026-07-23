using PaymentAPI.Interfaces;

namespace PaymentAPI.Services
{
    public class WebhookVerifierContext
    {
        private readonly Dictionary<string, ISourceIpVerifier> _sourceIpVerifiers;

        public WebhookVerifierContext(IEnumerable<ISourceIpVerifier> sourceIpVerifiers)
        {
            _sourceIpVerifiers = sourceIpVerifiers.ToDictionary(k => k.ProviderName, StringComparer.OrdinalIgnoreCase);
        }

        public Task<bool> VerifyAsync(string provider, HttpContext httpContext)
        {
            if (!_sourceIpVerifiers.TryGetValue(provider, out var sourceIpVerifier))
            {
                throw new ArgumentException($"Провайдер {provider} не поддерживается", nameof(provider));
            }

            return Task.FromResult(sourceIpVerifier.VerifySourceIp(httpContext));
        }
    }
}

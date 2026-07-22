using Microsoft.Extensions.Options;
using PaymentAPI.Interfaces;
using PaymentAPI.Settings;
using System.Net;

namespace PaymentAPI.Services
{
    public class YookassaIpVerifier : ISourceIpVerifier
    {
        public string ProviderName => "yookassa";
        private readonly YookassaSettings _settings;

        public YookassaIpVerifier(IOptions<YookassaSettings> options)
        {
            _settings = options.Value;
        }

        public bool VerifySourceIp(HttpContext httpContext)
        {
            var sourceIP = httpContext.Connection.RemoteIpAddress?.ToString();
            if (sourceIP == null) return false;
            return IsAllowedIP(sourceIP);
        }

        private bool IsAllowedIP(string stringSourceIP)
        {
            stringSourceIP = stringSourceIP.Replace("::ffff:", "");

            if (!IPAddress.TryParse(stringSourceIP, out var sourceIP))
                return false;

            foreach (var network in _settings.AllowedWebhooksIPs)
            {
                if (IPNetwork.TryParse(network, out var allowedNetwork)
                    && allowedNetwork.Contains(sourceIP))
                    return true;
            }
            return false;
        }
    }
}

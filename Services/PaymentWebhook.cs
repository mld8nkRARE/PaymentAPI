using PaymentAPI.DTO;
using PaymentAPI.Interfaces;
using System.Text.Json;
using System.Net;
using PaymentAPI.Settings;

namespace PaymentAPI.Services
{
    public class PaymentWebhook : IPaymentWebhook
    {
        private readonly YookassaSettings _settings;
        public PaymentWebhook(YookassaSettings settings)
        {
            _settings = settings;
        }
        public bool VerifyWebhookAsync(HttpContext httpContext)
        {
            var sourceIP = httpContext.Connection.RemoteIpAddress?.ToString();
            if (sourceIP == null) return false;
            return IsAllowedIP(sourceIP);
           
        }
        public WebhookData? ParseWebhookAsync(JsonElement bodyRequest)
        {
            return null;
        }

        public async Task<bool> ProcessWebhookAsync(WebhookData webhookData)  
        {
            return false;
        }

        private bool IsAllowedIP(string stringSourceIP)
        {
            stringSourceIP = stringSourceIP.Replace("::ffff:","");

            if (!IPAddress.TryParse(stringSourceIP, out var sourceIP))
            {
                return false;
            }
            foreach (var network in _settings.AllowedWebhooksIPs)
            {
                if (IPNetwork.TryParse(network, out var allowedNetwork))
                {
                    if (allowedNetwork.Contains(sourceIP))
                        return true;
                }
            }
            return false;
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography.X509Certificates;
using PaymentAPI.DTO;
using System.Text.Json;

namespace PaymentAPI.Interfaces
{
    public interface IPaymentWebhook
    {
        public bool VerifyWebhookAsync(HttpContext httpContext);
        public WebhookData? ParseWebhookAsync(JsonElement bodyRequest);
        public Task<bool> ProcessWebhookAsync(WebhookData webhookData);
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PaymentAPI.Services;
using System.Text.Json;

namespace PaymentAPI.Controllers
{
    [AllowAnonymous]
    [Route("api/payment_webhook")]
    [ApiController]
    public class PaymentWebhookController : ControllerBase
    {
        private readonly WebhookVerifierContext _webhookVerifierContext;
        private readonly WebhookHandler _webhookHandler;

        public PaymentWebhookController(
            WebhookVerifierContext webhookVerifierContext,
            WebhookHandler webhookHandler)
        {
            _webhookVerifierContext = webhookVerifierContext;
            _webhookHandler = webhookHandler;
        }

        [HttpPost("{provider}")]
        public async Task<IActionResult> HandleWebhook(string provider, [FromBody] JsonElement body)
        {
            if (!await _webhookVerifierContext.VerifyAsync(provider, HttpContext))
                return Unauthorized();

            await _webhookHandler.HandleAsync(provider, body);
            return Ok();
        }
    }
}

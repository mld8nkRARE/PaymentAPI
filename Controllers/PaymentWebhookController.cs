using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PaymentAPI.DTO;
using PaymentAPI.Services;
using System.Text;
using System.Text.Json;

namespace PaymentAPI.Controllers
{
    [AllowAnonymous]
    [Route("api/payment_webhook")]
    [ApiController]
    public class PaymentWebhookController : ControllerBase
    {
        private readonly WebhookParserContext _webhookParserContext;
        private readonly WebhookVerifierContext _webhookVerifierContext;
        private readonly WebhookHandler _webhookHandler;

        public PaymentWebhookController(
            WebhookParserContext webhookParserContext,
            WebhookVerifierContext webhookVerifierContext,
            WebhookHandler webhookHandler)
        {
            _webhookParserContext = webhookParserContext;
            _webhookVerifierContext = webhookVerifierContext;
            _webhookHandler = webhookHandler;
        }

        [HttpPost("{provider}")]
        public async Task<IActionResult> HandleWebhook(string provider)
        {
            HttpContext.Request.EnableBuffering();
            var rawBody = await ReadRawBodyAsync(Request);

            if (!await _webhookVerifierContext.VerifyAsync(provider, HttpContext, rawBody))
                return Unauthorized();

            var jsonElement = JsonSerializer.Deserialize<JsonElement>(rawBody);
            var webhookRequest = _webhookParserContext.Parse(provider, jsonElement);

            await _webhookHandler.HandleAsync(webhookRequest);
            return Ok();
        }

        private static async Task<string> ReadRawBodyAsync(HttpRequest request)
        {
            using var reader = new StreamReader(request.Body, Encoding.UTF8, leaveOpen: true);
            var rawBody = await reader.ReadToEndAsync();
            request.Body.Position = 0;
            return rawBody;
        }
    }
}

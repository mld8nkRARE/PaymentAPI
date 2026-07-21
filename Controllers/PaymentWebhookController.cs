using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PaymentAPI.DTO;
using PaymentAPI.Interfaces;
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
        public PaymentWebhookController(WebhookParserContext webhookParserContext, WebhookVerifierContext webhookVerifierContext)
        {
            _webhookParserContext = webhookParserContext;
            _webhookVerifierContext = webhookVerifierContext;
        }

        [HttpPost("{provider}")]
        public async Task<IActionResult> HandleWebhook(string provider)
        {
            try
            {
                HttpContext.Request.EnableBuffering();
                var rawBody = await ReadRawBodyAsync(Request);

                bool isVerifyingRequest = await _webhookVerifierContext.VerifyAsync(provider,HttpContext);
                if (!isVerifyingRequest)
                {
                    return Unauthorized();
                }
                return Ok();
            }
            catch
            {
                return BadRequest();
            }
        }
        private async static Task<string> ReadRawBodyAsync(HttpRequest request)
        {
            using var reader = new StreamReader(request.Body, Encoding.UTF8, leaveOpen:true);
            var rawBody = await reader.ReadToEndAsync();
            request.Body.Position = 0;
            return rawBody;
        }
    }
}

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

        /// <summary>
        /// Обрабатывает входящие вебхуки от платежных провайдеров.
        /// </summary>
        /// <param name="provider">Название платежного провайдера.</param>
        /// <param name="body">Тело запроса вебхука в формате JSON.</param>
        /// <returns>Статус обработки вебхука.</returns>
        [HttpPost("{provider}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> HandleWebhook(string provider, [FromBody] JsonElement body)
        {
            if (!await _webhookVerifierContext.VerifyAsync(provider, HttpContext))
                return Unauthorized();

            await _webhookHandler.HandleAsync(provider, body);
            return Ok();
        }
    }
}

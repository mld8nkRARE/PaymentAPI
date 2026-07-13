using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PaymentAPI.DTO;
using PaymentAPI.Interfaces;
using System.Text.Json;
namespace PaymentAPI.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentWebhookController : ControllerBase
    {
        private readonly IPaymentWebhook _paymentWebhook;
        PaymentWebhookController(IPaymentWebhook paymentWebhook)
        {
            _paymentWebhook = paymentWebhook;
        }
        [HttpPost]
        public async Task<IActionResult> HandleWebhook([FromBody] JsonElement bodyRequest)
        {
             
            try
            {
                bool isRequestFromVerifySource =  _paymentWebhook.VerifyWebhookAsync(HttpContext);
                if (!isRequestFromVerifySource)
                {
                    return Unauthorized();
                }
                WebhookData? webhookData =  _paymentWebhook.ParseWebhookAsync(bodyRequest);
                if(webhookData == null)
                {
                    return BadRequest();
                }
                await _paymentWebhook.ProcessWebhookAsync(webhookData);
                return Ok();
            }
            catch
            {
                return BadRequest();
            }
        }
    }
}

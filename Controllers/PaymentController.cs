using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PaymentAPI.DTO;
using PaymentAPI.Interfaces;

namespace PaymentAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentGateway _paymentGateway;

        public PaymentController(IPaymentGateway paymentGateway)
        {
            _paymentGateway = paymentGateway;
        }

        [HttpPost]
        public async Task<IActionResult> CreatePayment([FromBody] PaymentRequest request,
            [FromHeader(Name = "Idempotence-Key")]string idempotenceKey) //для idempotenceKey рекомендуется использовать V4 UUID
        {
            try
            {
                PaymentResult result = await _paymentGateway.CreatePayment(request, idempotenceKey);
                return Ok(result);
            }
            catch
            {
                return BadRequest();
            }
        }
        

    }
}

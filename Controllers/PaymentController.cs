using Microsoft.AspNetCore.Mvc;
using PaymentAPI.Interfaces;
using PaymentAPI.DTO;

namespace PaymentAPI.Controllers
{
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
        public async Task<IActionResult> CreatePayment([FromBody] PaymentRequest request, CancellationToken cancellationToken)
        {
            PaymentResult result = await _paymentGateway.CreatePayment(request);
            return Ok(result);

        }


    }
}

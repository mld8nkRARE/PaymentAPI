using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PaymentAPI.Application.Payments;
using PaymentAPI.DTO.payment;
using PaymentAPI.Extensions;

namespace PaymentAPI.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly PaymentHandler _handler;

        public PaymentController(PaymentHandler handler)
        {
            _handler = handler;
        }

        /// <summary>
        /// Создает новый платёж.
        /// </summary>
        /// <param name="request">Данные для создания платежа.</param>
        /// <param name="idempotenceKey">Ключ идемпотентности для запроса.</param>
        /// <returns>Результат создания платежа.</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PaymentResult))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreatePayment(
            [FromBody] PaymentCreateRequest request,
            [FromHeader(Name = "Idempotence-Key")] string idempotenceKey)
        {
            try
            {
                if (User.TryGetUserId(out var userId))
                {
                    var result = await _handler.CreatePaymentAsync(request, userId, idempotenceKey);
                    return Ok(result);
                }
                return BadRequest();
            }
            catch (NotSupportedException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}

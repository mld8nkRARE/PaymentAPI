using Microsoft.AspNetCore.Mvc;
using PaymentAPI.DTO;
using PaymentAPI.Models;
using PaymentAPI.Primitives;
using PaymentAPI.Services;

namespace PaymentAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly CreatePaymentHandler _handler;

        public PaymentController(CreatePaymentHandler handler)
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
                var userId = new UserId(Guid.Parse(User.FindFirst("sub")!.Value));
                var result = await _handler.HandleAsync(request, userId, idempotenceKey);
                return Ok(result);
            }
            catch (NotSupportedException ex)
            {
                return BadRequest(ex.Message);
            }
            catch
            {
                return BadRequest();
            }
        }
    }
}

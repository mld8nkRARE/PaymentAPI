using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PaymentAPI.DTO.refund;
using PaymentAPI.Models;
using PaymentAPI.Primitives;
using PaymentAPI.Services;
using System.Security.Claims;

namespace PaymentAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class RefundController : ControllerBase
    {
        private readonly RefundHandler _refundHandler;

        public RefundController(RefundHandler refundHandler)
        {
            _refundHandler = refundHandler;
        }

        /// <summary>
        /// Создает новый возврат.
        /// </summary>
        /// <param name="request">Данные для создания возврата.</param>
        /// <param name="idempotenceKey">Ключ идемпотентности для запроса.</param>
        /// <param name="provider">Платежный провайдер.</param>
        /// <returns>Созданный возврат.</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(RefundResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateRefund(
            [FromBody] RefundCreateRequest request,
            [FromHeader(Name = "Idempotence-Key")] string idempotenceKey,
            [FromHeader(Name = "Provider")] string provider)
        {
            try
            {
                var userId = GetUserId();
                var refund = await _refundHandler.CreateRefundAsync(request, userId, provider, idempotenceKey);
                return Ok(refund);
            }
            catch (NotSupportedException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception)
            {
                return BadRequest("Ошибка при создании возврата");
            }
        }

        /// <summary>
        /// Получает возврат по его идентификатору.
        /// </summary>
        /// <param name="id">Идентификатор возврата.</param>
        /// <returns>Возврат.</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(RefundResponse))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetRefund(Guid id)
        {
            try
            {
                var userId = GetUserId();
                var refund = await _refundHandler.GetRefundAsync(new RefundId(id), userId);

                if (refund is null)
                    return NotFound();

                return Ok(refund);
            }
            catch (Exception)
            {
                return BadRequest("Ошибка при получении информации о возврате");
            }
        }

        /// <summary>
        /// Получает возвраты, связанные с идентификатором платежа.
        /// </summary>
        /// <param name="paymentId">Идентификатор платежа.</param>
        /// <returns>Список возвратов.</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<RefundResponse>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetRefundsByPayment([FromQuery] Guid paymentId)
        {
            try
            {
                var userId = GetUserId();
                var refunds = await _refundHandler.GetRefundsByPaymentAsync(new PaymentId(paymentId), userId);
                return Ok(refunds);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception)
            {
                return BadRequest("Ошибка при получении списка возвратов");
            }
        }
        private UserId GetUserId()
        {
            var claim = User.FindFirst("sub")
                ?? throw new UnauthorizedAccessException();
            return new UserId(Guid.Parse(claim.Value));
        }
    }
}

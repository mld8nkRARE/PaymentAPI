using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PaymentAPI.DTO.order;
using PaymentAPI.Extensions;
using PaymentAPI.Models;
using PaymentAPI.Primitives;
using PaymentAPI.Services;


namespace PaymentAPI.Controllers
{
    [Authorize]
    [Route("api/orders")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly OrderService _orderService;

        public OrderController(OrderService orderService)
        {
            _orderService = orderService;
        }

        /// <summary>
        /// Создает новый заказ.
        /// </summary>
        /// <param name="request">Данные для создания заказа.</param>
        /// <returns>Созданный заказ.</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(OrderResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> CreateOrder([FromBody] OrderCreateRequest request)
        {
            //TO DO
            //идемпотентность
            var userId = GetUserId();
            var order = await _orderService.CreateOrderAsync(request, userId);
            return Ok(order);
        }

        /// <summary>
        /// Отменяет существующий заказ.
        /// </summary>
        /// <param name="id">Идентификатор заказа для отмены.</param>
        /// <returns>Статус выполнения операции.</returns>
        [HttpPost("{id}/cancel")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> CancelOrder(Guid id)
        {
            var userId = GetUserId();
            await _orderService.CancelOrderAsync(new OrderId(id), userId);
            return NoContent();
        }

        private UserId GetUserId()
        {
            if(!User.TryGetUserId(out var userId))
                throw new UnauthorizedAccessException();

            return userId;
        }
    }
}

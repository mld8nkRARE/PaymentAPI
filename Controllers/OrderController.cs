using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PaymentAPI.DTO;
using PaymentAPI.Primitives;
using PaymentAPI.Services;
using System.Security.Claims;

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

        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request)
        {
            var userId = GetUserId();
            var order = await _orderService.CreateOrderAsync(request, userId);
            return Ok(order);
        }

        [HttpPost("{id}/cancel")]
        public async Task<IActionResult> CancelOrder(Guid id)
        {
            var userId = GetUserId();
            await _orderService.CancelOrderAsync(new OrderId(id), userId);
            return NoContent();
        }

        private UserId GetUserId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier)
                ?? throw new UnauthorizedAccessException();
            return new UserId(Guid.Parse(claim.Value));
        }
    }
}

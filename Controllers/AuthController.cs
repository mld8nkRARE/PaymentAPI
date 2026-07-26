using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PaymentAPI.DTO;
using PaymentAPI.Interfaces;
using PaymentAPI.Models;

namespace PaymentAPI.Controllers
{
    [AllowAnonymous]   
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserAuthRequest userAuthRequest)
        {
            var token = await _authService.RegisterAsync(userAuthRequest);
            return Ok(token);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserAuthRequest userAuthRequest)
        {
            var token = await _authService.LoginAsync(userAuthRequest);
            return Ok(token);
        }
        [HttpPost("refresh")]
        public async Task<IActionResult> RefreshToken(UserRefreshAuthTokenRequest oldRefreshToken)
        {
            var token = await _authService.RefreshTokenAsync(oldRefreshToken);
            return Ok(token);
        }
    }
}

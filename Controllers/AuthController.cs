using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PaymentAPI.Application.Auth;
using PaymentAPI.DTO.auth;

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

        /// <summary>
        /// Регистрирует нового пользователя.
        /// </summary>
        /// <param name="userAuthRequest">Данные для регистрации пользователя.</param>
        /// <returns>Токен аутентификации.</returns>
        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AuthUserResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register([FromBody] AuthUserRequest userAuthRequest)
        {
            var token = await _authService.RegisterAsync(userAuthRequest);
            return Ok(token);
        }

        /// <summary>
        /// Выполняет вход пользователя и возвращает JWT-токен.
        /// </summary>
        /// <param name="userAuthRequest">Учетные данные пользователя.</param>
        /// <returns>JWT-токен.</returns>
        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login([FromBody] AuthUserRequest userAuthRequest)
        {
            var token = await _authService.LoginAsync(userAuthRequest);
            return Ok(token);
        }
        /// <summary>
        /// Обновляет JWT-токен с использованием токена обновления.
        /// </summary>
        /// <param name="oldRefreshToken">Запрос, содержащий старый токен обновления.</param>
        /// <returns>Новый JWT-токен.</returns>
        [HttpPost("refresh")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> RefreshToken(AuthRefreshRequest oldRefreshToken)
        {
            var token = await _authService.RefreshTokenAsync(oldRefreshToken);
            return Ok(token);
        }
    }
}

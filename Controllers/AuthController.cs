using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PaymentAPI.Application.Auth;
using PaymentAPI.DTO.auth;
using PaymentAPI.Extensions;
using PaymentAPI.Primitives;

namespace PaymentAPI.Controllers
{ 
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
        [AllowAnonymous]
        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AuthUserResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register([FromBody] AuthUserRequest userAuthRequest)
        {
            try
            {
                var token = await _authService.RegisterAsync(userAuthRequest);
                return Ok(token);
            }
            catch (InvalidDataException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Выполняет вход пользователя и возвращает JWT-токен.
        /// </summary>
        /// <param name="userAuthRequest">Учетные данные пользователя.</param>
        /// <returns>JWT-токен.</returns>
        [AllowAnonymous]
        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login([FromBody] AuthUserRequest userAuthRequest)
        {
            try
            {
                var token = await _authService.LoginAsync(userAuthRequest);
                return Ok(token);
            }
            catch (InvalidCredentialsException ex)
            {
                return Unauthorized(ex.Message);
            }
        }

        /// <summary>
        /// Обновляет JWT-токен с использованием токена обновления.
        /// </summary>
        /// <param name="authRefreshRequest">Запрос, содержащий старый токен обновления.</param>
        /// <returns>Новый JWT-токен.</returns>
        [AllowAnonymous]
        [HttpPost("refresh")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> RefreshToken(AuthRefreshRequest authRefreshRequest)
        {
            try
            {
                var tokens = await _authService.RefreshTokenAsync(authRefreshRequest);
                return Ok(tokens);
            }
            catch (RefreshTokenNotFoundException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (RefreshTokenExpiredException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (RefreshTokenReusedException ex)
            {
                return Unauthorized(ex.Message);
            }
        }

        /// <summary>
        /// Делает текущую сессию пользователя неактивной
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost("logout")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Logout([FromBody] AuthRefreshRequest request)
        {
            try
            {
                await _authService.LogoutAsync(request.RefreshToken);
                return Ok();
            }
            catch
            {
                return Unauthorized();
            }
        }
        /// <summary>
        /// Делает все сессии пользователя неактивными
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpPost("logout-all")]
        [ProducesResponseType(StatusCodes.Status200OK)] 
        public async Task<IActionResult> LogoutAll()
        {
            if (User.TryGetUserId(out var userId))
            {
                await _authService.LogoutAllAsync(userId);
                return Ok();
            }
            return Unauthorized();
        }

    }
}

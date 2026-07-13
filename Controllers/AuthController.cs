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
        //ILogger
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly IJwtService _jwtService;

        public AuthController(UserManager<User> userManager, SignInManager<User> signInManager,
            IConfiguration configuration, IJwtService jwtService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _jwtService = jwtService;
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody]UserAuthRequest userAuthRequest)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var user = new User(userAuthRequest.Email);
            var result = await _userManager.CreateAsync(user, userAuthRequest.Password);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors.Select(e => e.Description));
            }
            var token = _jwtService.GenerateToken(user);
            return Ok(token);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserAuthRequest userAuthRequest)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var user = await _userManager.FindByEmailAsync(userAuthRequest.Email);
            if (user is null)
            {
                return Unauthorized("Неверный Email или пароль");
            }
            var password = userAuthRequest.Password;
            var resultPasswordSign = await _signInManager.CheckPasswordSignInAsync(user, password, true);
            if (resultPasswordSign.Succeeded)
            {
                var token = _jwtService.GenerateToken(user);
                return Ok(token);
            }
            else
            {
                return Unauthorized("Неверный Email или пароль");
            }
        }
    }
}

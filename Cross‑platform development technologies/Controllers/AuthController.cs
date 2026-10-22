using Cross_platform_development_technologies.Contracts;
using Cross_platform_development_technologies.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cross_platform_development_technologies.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;
        private readonly TokenService _tokenService;

        public AuthController(
            AuthService authService,
            TokenService tokenService)
        {
            _authService = authService;
            _tokenService = tokenService;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult<LoginResponse>> Login(
            [FromBody] LoginRequest request)
        {
            var user = await _authService.ValidateCredentialsAsync(
                request.Login,
                request.Password);

            if (user is null)
            {
                return Unauthorized(new
                {
                    message = "Неверный логин или пароль."
                });
            }

            var token = _tokenService.CreateToken(user);

            return Ok(new LoginResponse
            {
                AccessToken = token,
                TokenType = "Bearer"
            });
        }
    }
}
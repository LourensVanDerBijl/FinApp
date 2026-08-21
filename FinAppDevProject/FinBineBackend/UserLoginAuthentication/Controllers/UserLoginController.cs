using Microsoft.AspNetCore.Mvc;
using FinBineBackend.UserLoginAuthentication.Models;
using FinBineBackend.UserLoginAuthentication.Services;

namespace FinBineBackend.UserLoginAuthentication.Controllers
{
    [ApiController]
    [Route("api/user/login")]
    public class UserLoginController : ControllerBase
    {
        private readonly UserLoginAuthService _loginService;

        public UserLoginController(UserLoginAuthService loginService)
        {
            _loginService = loginService;
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromBody] UserLoginRequest request)
        {
            if (string.IsNullOrEmpty(request.Token))
                return BadRequest(new UserLoginResponse { Success = false, Message = "Token is required." });

            string ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";

            var result = await _loginService.VerifyTokenAsync(request.Token, ipAddress);

            return result.Success ? Ok(result) : Unauthorized(result);
        }
    }
}
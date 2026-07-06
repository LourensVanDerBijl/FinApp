using Microsoft.AspNetCore.Mvc;
using FinBineBackend.AdminAuthentication.Models;
using FinBineBackend.AdminAuthentication.Services;

namespace FinBineBackend.AdminAuthentication.Controllers
{
    [ApiController]
    [Route("api/admin")]
    public class AdminAuthController : ControllerBase
    {
        private readonly AdminAuthService _authService;

        public AdminAuthController(AdminAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] AdminLoginRequest request)
        {
            if (string.IsNullOrEmpty(request.Token))
                return BadRequest(new { message = "Token is required" });

            var result = await _authService.VerifyTokenAsync(request.Token);

            if (!result.Success)
                return Unauthorized(result);

            return Ok(result);
        }
    }
}

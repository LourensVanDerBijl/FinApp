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

            // Grab the caller's IP address, so failed attempts can be
            // traced later if needed (e.g. reporting to a cyber crimes unit).
            string ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";

            var result = await _authService.VerifyTokenAsync(request.Token, ipAddress);

            if (!result.Success)
                return Unauthorized(result);

            return Ok(result);
        }
    }
}
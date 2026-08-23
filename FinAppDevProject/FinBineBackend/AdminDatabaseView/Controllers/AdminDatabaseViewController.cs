using Microsoft.AspNetCore.Mvc;
using FinBineBackend.AdminDatabaseView.Models;
using FinBineBackend.AdminDatabaseView.Services;

namespace FinBineBackend.AdminDatabaseView.Controllers
{
    // Backs the Admin > Development > DbUsers page. Every action takes
    // the admin's Firebase ID token in the body (same convention as
    // AdminAuthController/UserGroupRegistrationController) and is
    // re-verified against fb_admin_users on every single call — there's
    // no separate auth middleware guarding this route.
    [ApiController]
    [Route("api/admin/dev/db-users")]
    public class AdminDatabaseViewController : ControllerBase
    {
        private readonly AdminDatabaseViewService _service;

        public AdminDatabaseViewController(AdminDatabaseViewService service)
        {
            _service = service;
        }

        [HttpPost("list")]
        public async Task<IActionResult> List([FromBody] ListDbUsersRequest request)
        {
            if (string.IsNullOrEmpty(request.Token))
                return BadRequest(new ListDbUsersResponse { Success = false, Message = "Token is required." });

            string ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
            var result = await _service.ListDbUsersAsync(request.Token, ipAddress);

            return result.Success ? Ok(result) : Unauthorized(result);
        }

        [HttpPost("add")]
        public async Task<IActionResult> Add([FromBody] AddDbUserRequest request)
        {
            if (string.IsNullOrEmpty(request.Token))
                return BadRequest(new AddDbUserResponse { Success = false, Message = "Token is required." });

            string ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
            var result = await _service.AddDbUserAsync(request, ipAddress);

            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost("delete")]
        public async Task<IActionResult> Delete([FromBody] DeleteDbUserRequest request)
        {
            if (string.IsNullOrEmpty(request.Token))
                return BadRequest(new DeleteDbUserResponse { Success = false, Message = "Token is required." });

            string ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
            var result = await _service.DeleteDbUserAsync(request, ipAddress);

            return result.Success ? Ok(result) : BadRequest(result);
        }
    }
}

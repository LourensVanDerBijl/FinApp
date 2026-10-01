using Microsoft.AspNetCore.Mvc;
using FinBineBackend.AdminDatabaseView.Models;
using FinBineBackend.AdminDatabaseView.Services;

namespace FinBineBackend.AdminDatabaseView.Controllers
{
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

        // NEW
        [HttpPost("list-groups")]
        public async Task<IActionResult> ListGroups([FromBody] ListDbGroupsRequest request)
        {
            if (string.IsNullOrEmpty(request.Token))
                return BadRequest(new ListDbGroupsResponse { Success = false, Message = "Token is required." });

            string ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
            var result = await _service.ListDbGroupsAsync(request.Token, ipAddress);

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
using Microsoft.AspNetCore.Mvc;
using FinBineBackend.AdminGroupView.Models;
using FinBineBackend.AdminGroupView.Services;

namespace FinBineBackend.AdminGroupView.Controllers
{
    // Backs the Admin > Groups page. Same auth convention as every other
    // admin endpoint — the admin's Firebase ID token travels in the body
    // and is re-verified on every call.
    [ApiController]
    [Route("api/admin/groups")]
    public class AdminGroupViewController : ControllerBase
    {
        private readonly AdminGroupViewService _service;

        public AdminGroupViewController(AdminGroupViewService service)
        {
            _service = service;
        }

        [HttpPost("list")]
        public async Task<IActionResult> List([FromBody] ListGroupsRequest request)
        {
            if (string.IsNullOrEmpty(request.Token))
                return BadRequest(new ListGroupsResponse { Success = false, Message = "Token is required." });

            string ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
            var result = await _service.ListGroupsAsync(request.Token, ipAddress);

            return result.Success ? Ok(result) : Unauthorized(result);
        }

        // Powers the Dashboard's stat cards + subscription donut.
        [HttpPost("summary")]
        public async Task<IActionResult> Summary([FromBody] ListGroupsRequest request)
        {
            if (string.IsNullOrEmpty(request.Token))
                return BadRequest(new GroupSummaryResponse { Success = false, Message = "Token is required." });

            string ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
            var result = await _service.GetSummaryAsync(request.Token, ipAddress);

            return result.Success ? Ok(result) : Unauthorized(result);
        }
    }
}

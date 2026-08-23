using Microsoft.AspNetCore.Mvc;
using FinBineBackend.UserGroupRegistration.Models;
using FinBineBackend.UserGroupRegistration.Services;

namespace FinBineBackend.UserGroupRegistration.Controllers
{
    [ApiController]
    [Route("api/user/group-registration")]
    public class UserGroupRegistrationController : ControllerBase
    {
        private readonly UserGroupRegistrationService _registrationService;

        public UserGroupRegistrationController(UserGroupRegistrationService registrationService)
        {
            _registrationService = registrationService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateGroup([FromBody] CreateGroupRequest request)
        {
            if (string.IsNullOrEmpty(request.Token))
                return BadRequest(new CreateGroupResponse { Success = false, Message = "Token is required." });

            string ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";

            var result = await _registrationService.CreateGroupAsync(request, ipAddress);

            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost("join")]
        public async Task<IActionResult> JoinGroup([FromBody] JoinGroupRequest request)
        {
            if (string.IsNullOrEmpty(request.Token))
                return BadRequest(new JoinGroupResponse { Success = false, Message = "Token is required." });

            string ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";

            var result = await _registrationService.RequestToJoinGroupAsync(request, ipAddress);

            return result.Success ? Ok(result) : BadRequest(result);
        }
    }
}

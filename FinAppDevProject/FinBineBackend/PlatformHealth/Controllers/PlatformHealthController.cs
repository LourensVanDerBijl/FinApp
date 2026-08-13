using Microsoft.AspNetCore.Mvc;
using FinBineBackend.PlatformHealth.Services;

namespace FinBineBackend.PlatformHealth.Controllers
{
    [ApiController]
    [Route("api/platform-health")]
    public class PlatformHealthController : ControllerBase
    {
        private readonly PlatformHealthCheckService _checkService;

        public PlatformHealthController(PlatformHealthCheckService checkService)
        {
            _checkService = checkService;
        }

        // Returns whatever the last check found — instant, no waiting.
        [HttpGet("status")]
        public IActionResult GetStatus()
        {
            var statuses = PlatformHealthStore.GetAll().Select(s => new
            {
                name = s.Name,
                status = s.Status,
                responseTime = s.ResponseTimeMs,
                uptime = s.UptimeMinutes
            });

            return Ok(statuses);
        }

        // Triggers an immediate, fresh check — used by the dashboard's
        // refresh button — then returns the updated results.
        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh()
        {
            await _checkService.RunHealthChecksAsync();
            return GetStatus();
        }
    }
}
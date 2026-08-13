using System.Globalization;
using FinBineBackend.LoggingLogic.LogDbWrite.Services;
using FinBineBackend.LoggingLogic.Logs.Services;
using Microsoft.AspNetCore.Mvc;

namespace FinBineBackend.LoggingLogic.LogDbWrite.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class LogsController : ControllerBase
    {
        private readonly LoggingSystemLoggingService _systemLogger;

        public LogsController(LoggingSystemLoggingService systemLogger)
        {
            _systemLogger = systemLogger;
        }

        /// <summary>
        /// Get logs from the current SAST day. Pass "since" (an ISO 8601
        /// timestamp, taken from a previous response's "timestamp" field)
        /// to only receive logs newer than that.
        /// </summary>
        [HttpGet("live")]
        public IActionResult GetLiveLogs([FromQuery] string? since = null)
        {
            try
            {
                DateTime? sinceUtc = null;

                if (!string.IsNullOrEmpty(since))
                {
                    if (!DateTime.TryParse(since, null, DateTimeStyles.RoundtripKind, out var parsed))
                    {
                        return BadRequest(new { message = "Invalid 'since' timestamp format." });
                    }
                    sinceUtc = parsed.ToUniversalTime();
                }

                var logs = LogQueue.GetLiveLogs(sinceUtc)
                    .Select(log => new
                    {
                        id = log.Id,
                        timestamp = log.Timestamp.ToString("o"), // raw ISO timestamp — use this as the next "since" value
                        time = log.Timestamp.ToString("dd MMM yyyy HH:mm:ss"),
                        type = log.Type,
                        source = log.Source,
                        message = log.Message,
                        level = log.Level
                    });

                return Ok(logs);
            }
            catch (Exception ex)
            {
                _systemLogger.LogApiRequestFailed(ex);
                return StatusCode(500, new { message = "Failed to retrieve logs." });
            }
        }
    }
}
using Microsoft.AspNetCore.Mvc;
using FinBineBackend.AdminTestDataImport.Models;
using FinBineBackend.AdminTestDataImport.Services;

namespace FinBineBackend.AdminTestDataImport.Controllers
{
    // Backs the Admin > Development > Test Data page. Same auth
    // convention as AdminDatabaseViewController — the admin's Firebase
    // ID token travels in the body and is re-verified on every call.
    [ApiController]
    [Route("api/admin/dev/test-data")]
    public class AdminTestDataImportController : ControllerBase
    {
        private readonly AdminTestDataImportService _service;

        public AdminTestDataImportController(AdminTestDataImportService service)
        {
            _service = service;
        }

        [HttpPost("import")]
        public async Task<IActionResult> Import([FromBody] ImportTestDataRequest request)
        {
            if (string.IsNullOrEmpty(request.Token))
                return BadRequest(new ImportTestDataResponse { Success = false, Message = "Token is required." });

            string ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
            var result = await _service.ImportTestDataAsync(request, ipAddress);

            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost("clear-all")]
        public async Task<IActionResult> ClearAll([FromBody] ClearTestDataRequest request)
        {
            if (string.IsNullOrEmpty(request.Token))
                return BadRequest(new ClearTestDataResponse { Success = false, Message = "Token is required." });

            string ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
            var result = await _service.ClearAllTestDataAsync(request, ipAddress);

            return result.Success ? Ok(result) : BadRequest(result);
        }
    }
}
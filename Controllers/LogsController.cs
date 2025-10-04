using BlockedCountriesAPI.DTOs;
using BlockedCountriesAPI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BlockedCountriesAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LogsController: ControllerBase
    {
        private readonly IBlockedAttemptLogger _blockedAttemptLogger;
        private readonly ILogger<LogsController> _logger;

        public LogsController(IBlockedAttemptLogger blockedAttemptLogger, ILogger<LogsController> logger)
        {
            _blockedAttemptLogger = blockedAttemptLogger;
            _logger = logger;
        }

        [HttpGet("blocked-attempts")]
        public async Task<IActionResult> GetBlockedAttempts(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            if (page < 1) page = 1;
            if (pageSize < 1 || pageSize > 100) pageSize = 10;

            try
            {
                var logs = await _blockedAttemptLogger.GetLogsAsync(page, pageSize);

                var response = new PaginatedResponse<object>
                {
                    Page = page,
                    PageSize = pageSize,
                    Items = logs.Select(log => new
                    {
                        log.IpAddress,
                        log.Timestamp,
                        log.CountryCode,
                        log.IsBlocked,
                        log.UserAgent
                    }).ToList()
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving blocked attempts logs");
                return StatusCode(500, "Error retrieving logs");
            }
        }
    }
}


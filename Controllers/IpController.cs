using BlockedCountriesAPI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BlockedCountriesAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class IpController: ControllerBase
    {
        private readonly IGeolocationService _geolocationService;
        private readonly ICountryService _countryService;
        private readonly IBlockedAttemptLogger _blockedAttemptLogger;
        private readonly ILogger<IpController> _logger;

        public IpController(
            IGeolocationService geolocationService,
            ICountryService countryService,
            IBlockedAttemptLogger blockedAttemptLogger,
            ILogger<IpController> logger)
        {
            _geolocationService = geolocationService;
            _countryService = countryService;
            _blockedAttemptLogger = blockedAttemptLogger;
            _logger = logger;
        }

        [HttpGet("lookup")]
        public async Task<IActionResult> LookupIp([FromQuery] string ipAddress = null)
        {
            try
            {
                if (string.IsNullOrEmpty(ipAddress))
                {
                     ipAddress = await _geolocationService.GetCallerIpAsync();
                    //ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
                    //ipAddress = IpAddressHelper.GetClientIpAddress(HttpContext);


                    if (string.IsNullOrEmpty(ipAddress))
                    {
                        return BadRequest("Unable to determine IP address");
                    }
                }

                if (!_geolocationService.IsValidIpAddress(ipAddress))
                {
                    return BadRequest("Invalid IP address format");
                }

                var geolocation = await _geolocationService.GetGeolocationAsync(ipAddress);

                var response = new
                {
                    IpAddress = geolocation.Ip,
                    CountryCode = geolocation.CountryCode,
                    CountryName = geolocation.Location.CountryName,
                    Continent = geolocation.Location.ContinentName,
                    Region = geolocation.Location.StateProvince,
                    City = geolocation.Location.City,
                    Latitude = geolocation.Location.Latitude,
                    Longitude = geolocation.Location.Longitude,
                    IsEu = geolocation.Location.IsEu
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error looking up IP: {IpAddress}", ipAddress);
                return StatusCode(500, $"Error looking up IP: {ex.Message}");
            }
        }

        [HttpGet("check-block")]
        public async Task<IActionResult> CheckIfBlocked()
        {
            try
            {
                var ipAddress = await _geolocationService.GetCallerIpAsync();
                var userAgent = Request.Headers["User-Agent"].ToString();

                if (string.IsNullOrEmpty(ipAddress))
                {
                    return BadRequest("Unable to determine IP address");
                }
                
                var geolocation = await _geolocationService.GetGeolocationAsync(ipAddress);

                var isBlocked = await _countryService.IsCountryBlockedAsync(geolocation.CountryCode);

                // Log the attempt
                await _blockedAttemptLogger.LogAttemptAsync(ipAddress, geolocation.CountryCode, isBlocked, userAgent);

                var response = new
                {
                    IsBlocked = isBlocked,
                    CountryCode = geolocation.CountryCode,
                    CountryName = geolocation.Location.CountryName,
                    IpAddress = ipAddress
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking block status");
                return StatusCode(500, $"Error checking block status: {ex.Message}");
            }
        }
    }
}


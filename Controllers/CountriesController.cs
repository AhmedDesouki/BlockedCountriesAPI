using BlockedCountriesAPI.DTOs;
using BlockedCountriesAPI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BlockedCountriesAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CountriesController : ControllerBase
    {
        private readonly ICountryService _countryService;
        private readonly ILogger<CountriesController> _logger;

        public CountriesController(ICountryService countryService, ILogger<CountriesController> logger)
        {
            _countryService = countryService;
            _logger = logger;
        }

        //USIG DTO 
        [HttpPost("block")]
        public async Task<IActionResult> BlockCountry([FromBody] BlockCountryRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.CountryCode) || request.CountryCode.Length != 2)
            {
                return BadRequest("Invalid country code. Must be 2 characters.");
            }

            var result = await _countryService.BlockCountryAsync(request.CountryCode);

            return result ? Ok() : Conflict("Country already blocked");
        }



        /*[HttpPost("block")]
        public async Task<IActionResult> BlockCountry([FromBody] string countryCode)
        {
            if (string.IsNullOrEmpty(countryCode) || countryCode.Length != 2)
            {
                return BadRequest("Invalid country code. Must be 2 characters.");
            }

            var result = await _countryService.BlockCountryAsync(countryCode);

            return result ? Ok() : Conflict("Country already blocked or invalid");
        }*/

        [HttpDelete("block/{countryCode}")]
        public async Task<IActionResult> UnblockCountry(string countryCode)
        {
            if (string.IsNullOrEmpty(countryCode) || countryCode.Length != 2)
            {
                return BadRequest("Invalid country code. Must be 2 characters.");
            }

            var result = await _countryService.UnblockCountryAsync(countryCode);

            return result ? Ok() : NotFound("Country not found in blocked list");
        }

        [HttpGet("blocked")]
        public async Task<IActionResult> GetBlockedCountries(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string search = null)
        {
            if (page < 1) page = 1;
            if (pageSize < 1 || pageSize > 100) pageSize = 10;

            var countries = await _countryService.GetBlockedCountriesAsync(page, pageSize, search);

            var response = new PaginatedResponse<object>
            {
                Page = page,
                PageSize = pageSize,
                Items = countries.Select(c => new
                {
                    c.CountryCode,
                    c.BlockedAt,
                    c.IsTemporary,
                    c.ExpiresAt
                }).ToList()
            };

            return Ok(response);
        }

        [HttpPost("temporal-block")]
        public async Task<IActionResult> AddTemporalBlock([FromBody] TemporalBlockRequest request)
        {
            if (string.IsNullOrEmpty(request.CountryCode) || request.CountryCode.Length != 2)
            {
                return BadRequest("Invalid country code. Must be 2 characters.");
            }

            if (request.DurationMinutes < 1 || request.DurationMinutes > 1440)
            {
                return BadRequest("Duration must be between 1 and 1440 minutes (24 hours).");
            }

            var result = await _countryService.AddTemporalBlockAsync(request.CountryCode, request.DurationMinutes);

            return result ? Ok() : Conflict("Country already blocked or invalid");
        }
    }
}


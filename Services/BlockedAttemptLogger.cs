using BlockedCountriesAPI.Models;
using BlockedCountriesAPI.Repositories.Interfaces;
using BlockedCountriesAPI.Services.Interfaces;

namespace BlockedCountriesAPI.Services
{
    public class BlockedAttemptLogger : IBlockedAttemptLogger
    {
        
        private readonly IBlockedAttemptRepository _logRepository;
        private readonly ILogger<BlockedAttemptLogger> _logger;

        public BlockedAttemptLogger(IBlockedAttemptRepository logRepository, ILogger<BlockedAttemptLogger> logger)
        {
            _logRepository = logRepository;
            _logger = logger;
        }

        public async Task LogAttemptAsync(string ipAddress, string countryCode, bool isBlocked, string userAgent)
        {
            var logEntry = new BlockedAttemptLog
            {
                IpAddress = ipAddress,
                Timestamp = DateTime.UtcNow,
                CountryCode = countryCode,
                IsBlocked = isBlocked,
                UserAgent = userAgent
            };

            await _logRepository.AddAsync(logEntry);
            _logger.LogInformation("Logged blocked attempt: IP {IpAddress}, Country {CountryCode}, Blocked: {IsBlocked}",
                ipAddress, countryCode, isBlocked);
        }

        public async Task<IEnumerable<BlockedAttemptLog>> GetLogsAsync(int page, int pageSize)
        {
            return await _logRepository.GetPagedAsync(page, pageSize);
        }
    }
}


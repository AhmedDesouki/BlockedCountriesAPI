using BlockedCountriesAPI.Models;

namespace BlockedCountriesAPI.Services.Interfaces
{
    public interface IBlockedAttemptLogger
    {
        Task LogAttemptAsync(string ipAddress, string countryCode, bool isBlocked, string userAgent);
        Task<IEnumerable<BlockedAttemptLog>> GetLogsAsync(int page, int pageSize);
    }
}

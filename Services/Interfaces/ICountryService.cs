using BlockedCountriesAPI.Models;

namespace BlockedCountriesAPI.Services.Interfaces
{
    public interface ICountryService
    {
        Task<bool> BlockCountryAsync(string countryCode);
        Task<bool> UnblockCountryAsync(string countryCode);
        Task<bool> AddTemporalBlockAsync(string countryCode, int durationMinutes);
        Task<IEnumerable<CountryBlock>> GetBlockedCountriesAsync(int page, int pageSize, string search = null);
        Task<bool> IsCountryBlockedAsync(string countryCode);
        Task RemoveExpiredTemporalBlocksAsync();
    }
}

using BlockedCountriesAPI.Models;

namespace BlockedCountriesAPI.Repositories.Interfaces
{
    public interface ICountryRepository
    {
        Task<bool> AddAsync(CountryBlock countryBlock);
        Task<bool> RemoveAsync(string countryCode);
        Task<CountryBlock> GetByCodeAsync(string countryCode);
        Task<IEnumerable<CountryBlock>> GetAllAsync();
        Task<bool> ExistsAsync(string countryCode);
        Task RemoveExpiredTemporalBlocksAsync();
    }
}

using BlockedCountriesAPI.Models;
using BlockedCountriesAPI.Repositories.Interfaces;
using System.Collections.Concurrent;

namespace BlockedCountriesAPI.Repositories
{
    public class CountryRepository: ICountryRepository
    {
        private readonly ConcurrentDictionary<string, CountryBlock> _blockedCountries = new();

        public Task<bool> AddAsync(CountryBlock countryBlock)
        {
            var countryCode = countryBlock.CountryCode.ToUpper();
            return Task.FromResult(_blockedCountries.TryAdd(countryCode, countryBlock));
        }

        public Task<bool> RemoveAsync(string countryCode)
        {
            countryCode = countryCode.ToUpper();
            return Task.FromResult(_blockedCountries.TryRemove(countryCode, out _));
        }

        public Task<CountryBlock> GetByCodeAsync(string countryCode)
        {
            countryCode = countryCode.ToUpper();
            _blockedCountries.TryGetValue(countryCode, out var countryBlock);
            return Task.FromResult(countryBlock);
        }

        public Task<IEnumerable<CountryBlock>> GetAllAsync()
        {
            return Task.FromResult(_blockedCountries.Values.AsEnumerable());
        }

        public Task<bool> ExistsAsync(string countryCode)
        {
            countryCode = countryCode.ToUpper();
            return Task.FromResult(_blockedCountries.ContainsKey(countryCode));
        }

        public Task RemoveExpiredTemporalBlocksAsync()
        {
            var expiredBlocks = _blockedCountries
                .Where(x => x.Value.IsTemporary &&
                           x.Value.ExpiresAt.HasValue &&
                           x.Value.ExpiresAt < DateTime.UtcNow)
                .Select(x => x.Key)
                .ToList();

            foreach (var countryCode in expiredBlocks)
            {
                _blockedCountries.TryRemove(countryCode, out _);
            }

            return Task.CompletedTask;
        }
    }
}


using BlockedCountriesAPI.Models;
using BlockedCountriesAPI.Repositories.Interfaces;
using BlockedCountriesAPI.Services.Interfaces;

namespace BlockedCountriesAPI.Services
{
    public class CountryService:ICountryService
    {
        
        private readonly ICountryRepository _countryRepository;
        private readonly ILogger<CountryService> _logger;

        public CountryService(ICountryRepository countryRepository, ILogger<CountryService> logger)
        {
            _countryRepository = countryRepository;
            _logger = logger;
        }

        public async Task<bool> BlockCountryAsync(string countryCode)
        {
            if (!IsValidCountryCode(countryCode))
            {
                _logger.LogWarning("Invalid country code: {CountryCode}", countryCode);
                return false;
            }

            var existingBlock = await _countryRepository.GetByCodeAsync(countryCode);
            if (existingBlock != null)
            {
                _logger.LogWarning("Country {CountryCode} is already blocked", countryCode);
                return false;
            }

            var countryBlock = new CountryBlock
            {
                CountryCode = countryCode.ToUpper(),
                BlockedAt = DateTime.UtcNow,
                IsTemporary = false
            };

            var result = await _countryRepository.AddAsync(countryBlock);

            if (result)
            {
                _logger.LogInformation("Country {CountryCode} blocked successfully", countryCode);
            }

            return result;
        }

        public async Task<bool> UnblockCountryAsync(string countryCode)
        {
            if (!IsValidCountryCode(countryCode))
            {
                _logger.LogWarning("Invalid country code: {CountryCode}", countryCode);
                return false;
            }

            var result = await _countryRepository.RemoveAsync(countryCode);

            if (result)
            {
                _logger.LogInformation("Country {CountryCode} unblocked successfully", countryCode);
            }
            else
            {
                _logger.LogWarning("Country {CountryCode} not found in blocked list", countryCode);
            }

            return result;
        }

        public async Task<bool> AddTemporalBlockAsync(string countryCode, int durationMinutes)
        {
            if (!IsValidCountryCode(countryCode))
            {
                _logger.LogWarning("Invalid country code: {CountryCode}", countryCode);
                return false;
            }

            if (durationMinutes < 1 || durationMinutes > 1440)
            {
                _logger.LogWarning("Invalid duration: {DurationMinutes} minutes", durationMinutes);
                return false;
            }

            var existingBlock = await _countryRepository.GetByCodeAsync(countryCode);
            if (existingBlock != null)
            {
                _logger.LogWarning("Country {CountryCode} is already blocked", countryCode);
                return false;
            }

            var countryBlock = new CountryBlock
            {
                CountryCode = countryCode.ToUpper(),
                BlockedAt = DateTime.UtcNow,
                IsTemporary = true,
                ExpiresAt = DateTime.UtcNow.AddMinutes(durationMinutes)
            };

            var result = await _countryRepository.AddAsync(countryBlock);

            if (result)
            {
                _logger.LogInformation("Country {CountryCode} temporarily blocked for {DurationMinutes} minutes",
                    countryCode, durationMinutes);
            }

            return result;
        }

        public async Task<IEnumerable<CountryBlock>> GetBlockedCountriesAsync(int page, int pageSize, string search = null)
        {
            var allCountries = await _countryRepository.GetAllAsync();
            var query = allCountries.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                search = search.ToUpper();
                query = query.Where(x => x.CountryCode.Contains(search));
            }

            return query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();
        }

        public async Task<bool> IsCountryBlockedAsync(string countryCode)
        {
            if (!IsValidCountryCode(countryCode))
                return false;

            var countryBlock = await _countryRepository.GetByCodeAsync(countryCode);

            if (countryBlock == null)
                return false;

            // Check if temporal block has expired
            if (countryBlock.IsTemporary && countryBlock.ExpiresAt.HasValue && countryBlock.ExpiresAt < DateTime.UtcNow)
            {
                await _countryRepository.RemoveAsync(countryCode);
                return false;
            }

            return true;
        }

        public async Task RemoveExpiredTemporalBlocksAsync()
        {
            await _countryRepository.RemoveExpiredTemporalBlocksAsync();
        }

        private bool IsValidCountryCode(string countryCode)
        {
            return !string.IsNullOrEmpty(countryCode) && countryCode.Length == 2;
        }
    }
}


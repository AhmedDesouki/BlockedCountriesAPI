using BlockedCountriesAPI.Models;

namespace BlockedCountriesAPI.Repositories.Interfaces
{
    public interface IBlockedAttemptRepository
    {
        Task AddAsync(BlockedAttemptLog log);
        Task<IEnumerable<BlockedAttemptLog>> GetPagedAsync(int page, int pageSize);
        Task<int> GetTotalCountAsync();
    }
}

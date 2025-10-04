using BlockedCountriesAPI.Models;
using BlockedCountriesAPI.Repositories.Interfaces;
using System.Collections.Concurrent;

namespace BlockedCountriesAPI.Repositories
{
    public class BlockedAttemptRepository : IBlockedAttemptRepository
    {
         
        private readonly ConcurrentBag<BlockedAttemptLog> _logs = new();

        public Task AddAsync(BlockedAttemptLog log)
        {
            _logs.Add(log);
            return Task.CompletedTask;
        }

        public Task<IEnumerable<BlockedAttemptLog>> GetPagedAsync(int page, int pageSize)
        {
            var logs = _logs
                .OrderByDescending(x => x.Timestamp)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .AsEnumerable();

            return Task.FromResult(logs);
        }

        public Task<int> GetTotalCountAsync()
        {
            return Task.FromResult(_logs.Count);
        }
    }
}


using BlockedCountriesAPI.Services.Interfaces;

namespace BlockedCountriesAPI.BackgroundServices
{
    public class TemporalBlockCleanupService :BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<TemporalBlockCleanupService> _logger;

        public TemporalBlockCleanupService(IServiceProvider serviceProvider, ILogger<TemporalBlockCleanupService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Temporal Block Cleanup Service started");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var countryService = scope.ServiceProvider.GetRequiredService<ICountryService>();
                        await countryService.RemoveExpiredTemporalBlocksAsync();
                        _logger.LogDebug("Expired temporal blocks cleaned up");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error cleaning up expired temporal blocks");
                }

                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
            }
        }
    }
}


using FinBineBackend.PlatformHealth.Services;

namespace FinBineBackend.PlatformHealth.BackgroundWorkers
{
    public class PlatformHealthBackgroundWorker : BackgroundService
    {
        private static readonly TimeSpan CheckInterval = TimeSpan.FromMinutes(2);
        private readonly IServiceScopeFactory _scopeFactory;

        public PlatformHealthBackgroundWorker(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var checkService = scope.ServiceProvider.GetRequiredService<PlatformHealthCheckService>();
                    await checkService.RunHealthChecksAsync();
                }

                await Task.Delay(CheckInterval, stoppingToken);
            }
        }
    }
}
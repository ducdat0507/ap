using Microsoft.AspNetCore.SignalR;
using System.Diagnostics;
using RemoteCheckup.Hubs;

namespace RemoteCheckup.Services
{
    public class PeriodicPerformanceCheckupService : BackgroundService
    {
        private readonly IHubContext<PerformanceCheckupHub> _hubContext;
        private readonly ILogger<PeriodicPerformanceCheckupService> _logger;

        public PeriodicPerformanceCheckupService(IHubContext<PerformanceCheckupHub> hubContext, ILogger<PeriodicPerformanceCheckupService> logger)
        {
            _hubContext = hubContext;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Periodic performance checkup service started");

            using PeriodicTimer timer = new(TimeSpan.FromSeconds(3));

            try
            {
                while (await timer.WaitForNextTickAsync(stoppingToken))
                {
                    await DoCheckup().ConfigureAwait(false);
                }
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning("Periodic performance checkup service stopped");
            }
        }

        protected async Task DoCheckup()
        {
            string message = "Performance not available";
            if (OperatingSystem.IsWindows()) // standard guard examples
            {
                message = string.Join("\n", PerformanceCounterCategory.GetCategories().Select(x => x.CategoryName));
            }

            // _logger.LogInformation(message);
            await _hubContext.Clients.All.SendAsync("update", message);
        }
    }
}

using Microsoft.AspNetCore.SignalR;
using System.Diagnostics;
using RemoteCheckup.Hubs;
using Microsoft.VisualBasic;
using RemoteCheckup.Models;
using System.Text.Json;
using RemoteCheckup.SubServices;

namespace RemoteCheckup.Services
{
    public class PeriodicPerformanceCheckupService : BackgroundService
    {
        private readonly IHubContext<PerformanceCheckupHub> _hubContext;
        private readonly ILogger<PeriodicPerformanceCheckupService> _logger;

        private PerformanceCheckupSubService subService;

        public PeriodicPerformanceCheckupService(IHubContext<PerformanceCheckupHub> hubContext, ILogger<PeriodicPerformanceCheckupService> logger)
        {
            _hubContext = hubContext;
            _logger = logger;

            if (OperatingSystem.IsWindows()) subService = new PerformanceCheckupOnWindowsSubService();
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
            PerformanceInfo? info = subService?.GetPerformanceInfo();
            await SetCurrentInfo(info);
        }

        protected async Task SetCurrentInfo(PerformanceInfo? info)
        {
            PerformanceCheckupHub.CurrentInfo = info;
            await _hubContext.Clients.All.SendAsync("update", info);
        }
    }
}

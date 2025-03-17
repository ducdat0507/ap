using Microsoft.AspNetCore.SignalR;
using System.Diagnostics;
using RemoteCheckup.Hubs;
using Microsoft.VisualBasic;
using RemoteCheckup.Models;

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
            PerformanceInfo? info = null;
            if (OperatingSystem.IsWindows()) // standard guard examples
            {
                var cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
                var ramCounter = new PerformanceCounter("Memory", "Available MBytes");
                info = new();
                info.Processors.Add(new () {
                    Usage = {cpuCounter.NextValue()},
                });
            }

            _logger.LogInformation(info?.ToString() ?? null);
            await _hubContext.Clients.All.SendAsync("update", info);
        }
    }
}

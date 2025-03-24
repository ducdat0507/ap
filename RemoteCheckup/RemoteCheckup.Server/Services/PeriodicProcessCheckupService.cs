using Microsoft.AspNetCore.SignalR;
using System.Diagnostics;
using RemoteCheckup.Hubs;
using Microsoft.VisualBasic;
using RemoteCheckup.DTOs;
using System.Text.Json;
using RemoteCheckup.Probes;

namespace RemoteCheckup.Services
{
    public class PeriodicProcessCheckupService : BackgroundService
    {
        private readonly IHubContext<ProcessesCheckupHub> _hubContext;
        private readonly ILogger<PeriodicProcessCheckupService> _logger;

        private ProcessCheckupProbe subService;

        public PeriodicProcessCheckupService(IHubContext<ProcessesCheckupHub> hubContext, ILogger<PeriodicProcessCheckupService> logger)
        {
            _hubContext = hubContext;
            _logger = logger;

            // if (OperatingSystem.IsWindows()) subService = new PerformanceCheckupOnWindowsSubService();
            /*else*/ if (OperatingSystem.IsLinux()) subService = new ProcessCheckupOnLinuxProbe();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Periodic processes checkup service started");

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
                _logger.LogWarning("Periodic processes checkup service stopped");
            }
        }

        protected async Task DoCheckup()
        {
            try
            {
                ProcessesInfo? info = subService?.GetProcessesInfo();
                await SetCurrentInfo(info);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Process checkup failed");
            }
        }

        protected async Task SetCurrentInfo(ProcessesInfo? info)
        {
            ProcessesCheckupHub.CurrentInfo = info;
            await _hubContext.Clients.All.SendAsync("update", info);
        }
    }
}

using Microsoft.AspNetCore.SignalR;
using System.Diagnostics;
using RemoteCheckup.Hubs;
using Microsoft.VisualBasic;
using RemoteCheckup.DTOs;
using System.Text.Json;
using RemoteCheckup.Probes;

namespace RemoteCheckup.Services
{
    public class PeriodicDatabaseCheckupService : BackgroundService
    {
        private readonly IHubContext<DatabasesCheckupHub> _hubContext;
        private readonly ILogger<PeriodicDatabaseCheckupService> _logger;

        private List<DatabaseCheckupProbe> probes;

        public PeriodicDatabaseCheckupService(IHubContext<DatabasesCheckupHub> hubContext, ILogger<PeriodicDatabaseCheckupService> logger)
        {
            _hubContext = hubContext;
            _logger = logger;

            // TODO allow configuration of this list
            probes = [
                new MySqlDatabaseCheckupProbe("server=localhost;username=root;password=;")
            ];
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Periodic database checkup service started");

            using PeriodicTimer timer = new(TimeSpan.FromSeconds(15));

            try
            {
                while (await timer.WaitForNextTickAsync(stoppingToken))
                {
                    await DoCheckup().ConfigureAwait(false);
                }
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning("Periodic database checkup service stopped");
            }
        }

        protected async Task DoCheckup()
        {
            try
            {
                DatabasesInfo info = new();
                List<Task> tasks = [];
                foreach (DatabaseCheckupProbe probe in probes) 
                {
                    DatabaseServerInfo dbsInfo = new ();
                    info.Servers.Add(dbsInfo);
                    tasks.Add(probe.GetDatabaseInfoAsync(dbsInfo));
                }
                await Task.WhenAll(tasks);
                await SetCurrentInfo(info);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Database checkup failed");
            }
        }

        protected async Task SetCurrentInfo(DatabasesInfo? info)
        {
            DatabasesCheckupHub.CurrentInfo = info;
            await _hubContext.Clients.All.SendAsync("update", info);
        }
    }
}

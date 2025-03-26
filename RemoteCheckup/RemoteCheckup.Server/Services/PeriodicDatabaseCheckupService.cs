using Microsoft.AspNetCore.SignalR;
using System.Diagnostics;
using RemoteCheckup.Hubs;
using Microsoft.VisualBasic;
using RemoteCheckup.DTOs;
using System.Text.Json;
using RemoteCheckup.Probes;
using RemoteCheckup.Models;

namespace RemoteCheckup.Services
{
    public class PeriodicDatabaseCheckupService : BackgroundService
    {
        private readonly IHubContext<DatabasesCheckupHub> _hubContext;
        private readonly ILogger<PeriodicDatabaseCheckupService> _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        private List<DatabaseCheckupProbe> probes = [];

        public PeriodicDatabaseCheckupService(
            IHubContext<DatabasesCheckupHub> hubContext, 
            ILogger<PeriodicDatabaseCheckupService> logger,
            IServiceScopeFactory serviceScopeFactory
        ) {
            _hubContext = hubContext;
            _logger = logger;
            _serviceScopeFactory = serviceScopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Periodic database checkup service started");

            UpdateDbProbes();
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

        public void UpdateDbProbes() {
            foreach (var oldProbe in probes) {
                oldProbe.Dispose();
            }

            using var scope = _serviceScopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            probes = [];
            foreach (var dbTarget in dbContext.DatabaseTargets)
            {
                probes.Add(DatabaseCheckupProbeFactory.Create(dbTarget));
            }
            _logger.LogInformation("Using {count} database probes", probes.Count);
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

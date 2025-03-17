using Microsoft.AspNetCore.SignalR;

namespace RemoteCheckup.Hubs
{
    public class PerformanceCheckupHub : Hub
    {
        private readonly ILogger<PerformanceCheckupHub> _logger;

        public PerformanceCheckupHub(ILogger<PerformanceCheckupHub> logger)
        {
            _logger = logger;
        }

        public async override Task OnConnectedAsync()
        {
            _logger.LogInformation("Client connected to hub");
            await Clients.All.SendAsync("update", "Hub connected");
            await base.OnConnectedAsync();
        }
    }
}

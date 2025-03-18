using Microsoft.AspNetCore.SignalR;
using RemoteCheckup.Models;

namespace RemoteCheckup.Hubs
{
    public class PerformanceCheckupHub : Hub
    {
        private readonly ILogger<PerformanceCheckupHub> _logger;
        public static PerformanceInfo? CurrentInfo { get; set; }

        public PerformanceCheckupHub(ILogger<PerformanceCheckupHub> logger)
        {
            _logger = logger;
        }


        public async override Task OnConnectedAsync()
        {
            _logger.LogInformation("Client connected to hub");
            await Clients.Caller.SendAsync("update", CurrentInfo);
            await base.OnConnectedAsync();
        }
    }
}

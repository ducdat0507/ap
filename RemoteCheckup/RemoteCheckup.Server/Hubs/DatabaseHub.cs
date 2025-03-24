using Microsoft.AspNetCore.SignalR;
using RemoteCheckup.DTOs;

namespace RemoteCheckup.Hubs
{
    public class DatabasesCheckupHub : Hub
    {
        private readonly ILogger<DatabasesCheckupHub> _logger;
        public static DatabasesInfo? CurrentInfo { get; set; }

        public DatabasesCheckupHub(ILogger<DatabasesCheckupHub> logger)
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

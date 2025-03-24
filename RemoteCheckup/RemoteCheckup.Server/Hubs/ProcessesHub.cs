using Microsoft.AspNetCore.SignalR;
using RemoteCheckup.DTOs;

namespace RemoteCheckup.Hubs
{
    public class ProcessesCheckupHub : Hub
    {
        private readonly ILogger<ProcessesCheckupHub> _logger;
        public static ProcessesInfo? CurrentInfo { get; set; }

        public ProcessesCheckupHub(ILogger<ProcessesCheckupHub> logger)
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

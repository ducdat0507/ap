using Microsoft.AspNetCore.SignalR;

namespace RemoteCheckup.Hubs
{
    public class PerformanceCheckupHub : Hub
    {
        public async override Task OnConnectedAsync()
        {
            Console.WriteLine("Client connected to hub");
            await Clients.All.SendAsync("update", "Hub connected");
        }
    }
}

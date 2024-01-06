using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace SignalRServer
{
    public class SampleHub : Hub
    {

        public override async Task OnConnectedAsync()
        {
            var userId = Context.User.Identity.Name;

            await Groups.AddToGroupAsync(Context.ConnectionId, userId);
            
            Console.WriteLine($"User {userId} connected.");

            await base.OnConnectedAsync();
        }

        public void SendMessageToServer(string message)
        {
            Console.WriteLine($"Received message from client: {message}");

        }
    }
}

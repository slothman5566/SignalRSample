using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace SignalRServer
{
    public class SampleHub : Hub
    {
        private readonly CurrentClientService _currentClientService;
        public SampleHub(CurrentClientService currentClientService)
        {
            _currentClientService = currentClientService;
        }

        public override async Task OnConnectedAsync()
        {
            var userId = Context.User.Identity.Name;

            await Groups.AddToGroupAsync(Context.ConnectionId, userId);

            Console.WriteLine($"User {userId} connected.");
            _currentClientService.Push(userId);
            await base.OnConnectedAsync();
        }

        public void SendMessageToServer(string message)
        {
            Console.WriteLine($"Received message from client: {message}");
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = Context.User.Identity.Name;
            Console.WriteLine($"User {userId} disconnected.");
            _currentClientService.Remove(userId);
            return base.OnDisconnectedAsync(exception);
        }
    }
}

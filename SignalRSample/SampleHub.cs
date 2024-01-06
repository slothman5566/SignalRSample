using Microsoft.AspNetCore.SignalR;

namespace SignalRServer
{
    public class SampleHub : Hub
    {
        public void SendMessageToServer(string message)
        {
            Console.WriteLine($"Received message from client: {message}");

        }
    }
}

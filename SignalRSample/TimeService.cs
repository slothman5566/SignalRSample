using Microsoft.AspNetCore.SignalR;

namespace SignalRServer
{
    public class TimeService : IHostedService, IDisposable
    {
        private readonly IHubContext<SampleHub> _hubContext;
        private Timer _timer;

        private readonly CurrentClientService _currentClientService;

        public TimeService(IHubContext<SampleHub> hubContext, CurrentClientService currentClientService)
        {
            _hubContext = hubContext;
            _currentClientService = currentClientService;
        }

        public Task StartAsync(CancellationToken stoppingToken)
        {
            _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromSeconds(1));
            return Task.CompletedTask;
        }

        private void DoWork(object state)
        {
            foreach (var userId in _currentClientService.GetAll())
            {
                var message = $"Hello from timed service {DateTime.UtcNow}";
                _hubContext.Clients.Group(userId).SendAsync("TimedService", message);
            }
        }

        public Task StopAsync(CancellationToken stoppingToken)
        {
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}

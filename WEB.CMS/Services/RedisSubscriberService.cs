using Entities.ViewModels.Car;
using Microsoft.AspNetCore.SignalR;

namespace WEB.CMS.Services
{
    public class RedisSubscriberService : BackgroundService
    {
        private readonly RedisConn _redisService;
        private readonly IHubContext<CarHub> _hubContext;

        public RedisSubscriberService(RedisConn redisService, IHubContext<CarHub> hubContext)
        {
            _redisService = redisService;
            _hubContext = hubContext;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _redisService.Connect();

            await _redisService.SubscribeAsync("ReceiveRegistration", async (RegistrationRecord record) =>
            {
                // Forward tới tất cả client qua SignalR
                await _hubContext.Clients.All.SendAsync("ReceiveRegistration", record);
            });

            await Task.CompletedTask;
        }
    }
}

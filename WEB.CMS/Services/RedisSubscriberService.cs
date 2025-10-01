using Entities.ViewModels.Car;
using Microsoft.AspNetCore.SignalR;
using Repositories.IRepositories;
using Repositories.Repositories;

namespace WEB.CMS.Services
{
    public class RedisSubscriberService : BackgroundService
    {
        private readonly RedisConn _redisService;
        private readonly IHubContext<CarHub> _hubContext;
        private readonly IVehicleInspectionRepository _vehicleInspectionRepository;

        public RedisSubscriberService(RedisConn redisService, IHubContext<CarHub> hubContext, IVehicleInspectionRepository vehicleInspectionRepository)
        {
            _redisService = redisService;
            _hubContext = hubContext;
            _vehicleInspectionRepository = vehicleInspectionRepository;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _redisService.Connect();

            await _redisService.SubscribeAsync("ReceiveRegistration", async (RegistrationRecord record) =>
            {
                // Forward tới tất cả client qua SignalR
                var id = _vehicleInspectionRepository.SaveVehicleInspection(record);
                
                if(id > 0)
                {
                    record.Id = id;
                    record.CreateTime = record.RegistrationTime.ToString("dd/MM/yyyy HH:mm:ss");
                    await _hubContext.Clients.All.SendAsync("ReceiveRegistration", record);
                }
                
            });

            await Task.CompletedTask;
        }
    }
}

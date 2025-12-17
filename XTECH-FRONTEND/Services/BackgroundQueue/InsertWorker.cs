using Google.Apis.Sheets.v4.Data;
using System.Text;
using XTECH_FRONTEND.IRepositories;
using XTECH_FRONTEND.Model;
using XTECH_FRONTEND.Utilities;

namespace XTECH_FRONTEND.Services.BackgroundQueue
{
    public class InsertWorker : BackgroundService
    {
        private readonly IInsertQueue _queue;
        private readonly IServiceProvider _serviceProvider;
        private readonly IHttpClientFactory _factory;
        private readonly IMongoService _mongoService;

        public InsertWorker(
            IInsertQueue queue,
            IServiceProvider serviceProvider,
            IHttpClientFactory factory,
            IMongoService mongoService)
        {
            _queue = queue;
            _serviceProvider = serviceProvider;
            _factory = factory;
            _mongoService = mongoService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var job = await _queue.DequeueAsync(stoppingToken);

                // chạy async tách riêng -> không block worker
                _ = Task.Run(() => ProcessJob(job, stoppingToken), stoppingToken);
            }
        }

        private async Task ProcessJob(InsertJob job, CancellationToken token)
        {
            try
            {
                var registrationRecord = new RegistrationRecord
                {
                    PhoneNumber = job.Data.PhoneNumber,
                    PlateNumber = job.Data.PlateNumber.ToUpper(),
                    Name = job.Data.Name.ToUpper(),
                    Referee = job.Data.Referee.ToUpper(),
                    GPLX = job.Data.GPLX.ToUpper(),
                    QueueNumber = (int)job.Data.QueueNumber,
                    RegistrationTime = (DateTime)job.Data.RegistrationTime,
                    ZaloStatus = "Đang xử lý...",
                    Camp = job.Data.Camp,
                };
                var InsertMG = await _mongoService.Insert(registrationRecord);
                if (InsertMG == 0)
                {
                    InsertMG = await _mongoService.Insert(registrationRecord);
                }
                using var scope = _serviceProvider.CreateScope();
                var logger = scope.ServiceProvider.GetRequiredService<ILogger<InsertWorker>>();

                var client = _factory.CreateClient("InsertClient");

                var json = Newtonsoft.Json.JsonConvert.SerializeObject(job.Data);

                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync(
                    "https://api-cargillhanam.adavigo.com/api/vehicleInspection/insert",
                    content,
                    token
                );

                if (!response.IsSuccessStatusCode)
                {
                    logger.LogError("InsertWorker: Failed to call API");
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("InsertWorker error: " + ex.Message);
            }
        }
    }

}

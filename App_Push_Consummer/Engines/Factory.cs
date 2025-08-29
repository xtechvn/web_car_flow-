using App_Push_Consummer.Behaviors;
using App_Push_Consummer.Common;
using App_Push_Consummer.Interfaces;
using App_Push_Consummer.Model;
using App_Push_Consummer.Redis;
using Google.Apis.Sheets.v4.Data;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Configuration;

namespace App_Push_Consummer.Engines
{
    public class Factory : IFactory
    {
        public static string tele_token = ConfigurationManager.AppSettings["tele_token"];
        public static string tele_group_id = ConfigurationManager.AppSettings["tele_group_id"];
        private readonly IGoogleSheetsService _googleSheetsService;
        private readonly RedisConn redisService;
        private readonly IMongoService _mongoService;


        public Factory(IGoogleSheetsService googleSheetsService, IMongoService mongoService)
        {
            _googleSheetsService = googleSheetsService;
            redisService = new RedisConn();
            redisService.Connect();
            _mongoService = mongoService;
        }

        public async void DoSomeRealWork(string data_queue)
        {
            try
            {
                var queue_info = JsonConvert.DeserializeObject<RegistrationRecord>(data_queue);
                if (queue_info != null)
                {
                    string cache_name = "PlateNumber_" + queue_info.PlateNumber.Replace("-", "_");
                    queue_info.QueueNumber = await _googleSheetsService.GetDailyQueueCountRedis();
                    redisService.Set(cache_name, JsonConvert.SerializeObject(queue_info), DateTime.Now.AddMinutes(15), Convert.ToInt32(ConfigurationManager.AppSettings["Redis_db_common"]));
                    _mongoService.Insert(queue_info);
                    DateTime now = DateTime.Now;
                    DateTime expireAt = new DateTime(now.Year, now.Month, now.Day, 18, 0, 0);
                    int hours = now.Hour;
                    int Minute = now.Minute;
                    if (hours == 18 && Minute < 30)
                    {
                        
                    }
                    else
                    {
                        var sheetsSuccess = await _googleSheetsService.SaveRegistrationAsync(queue_info);
                        Console.WriteLine($"lưu thành công: {sheetsSuccess}");
                        if (!sheetsSuccess)
                        {

                            ErrorWriter.InsertLogTelegramByUrl(tele_token, tele_group_id, "Lưu ex ko thành công = " + queue_info.ToString());
                            _googleSheetsService.SaveRegistrationAsync(queue_info);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                ErrorWriter.InsertLogTelegramByUrl(tele_token, tele_group_id, "DoSomeRealWork = " + ex.ToString());
            }
        }
        public async void UpdateEx()
        {
            try
            {
                var data = _mongoService.GetList();
                _googleSheetsService.SaveRegistrationEX(data);
            }
            catch (Exception ex)
            {
                ErrorWriter.InsertLogTelegramByUrl(tele_token, tele_group_id, "UpdateEx = " + ex.ToString());
            }
        }
    }
}
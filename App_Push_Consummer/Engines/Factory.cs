using App_Push_Consummer.Behaviors;
using App_Push_Consummer.Common;
using App_Push_Consummer.Interfaces;
using App_Push_Consummer.Model;
using App_Push_Consummer.Redis;
using Google.Apis.Sheets.v4.Data;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Configuration;
using System.Diagnostics;

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

                    queue_info.RegistrationTime = DateTime.Now;
                    DateTime now = DateTime.Now;
                    string cache_name = "PlateNumber_" + queue_info.PlateNumber.Replace("-", "_");
                    string cache_name2 = "PlateNumber_" + queue_info.PlateNumber.Replace("-", "_") + "_" + now.ToString("dd_MM_yyyy");
                    //queue_info.QueueNumber = await _googleSheetsService.GetDailyQueueCountRedis();
                    redisService.Set(cache_name2, JsonConvert.SerializeObject(queue_info), DateTime.Now.AddDays(1), Convert.ToInt32(ConfigurationManager.AppSettings["Redis_db_common"]));
                    redisService.Set(cache_name, JsonConvert.SerializeObject(queue_info), DateTime.Now.AddMinutes(15), Convert.ToInt32(ConfigurationManager.AppSettings["Redis_db_common"]));

                    var insertResult = await _mongoService.Insert(queue_info);
                    if (insertResult == 0)
                    {
                        insertResult = await _mongoService.Insert(queue_info);
                    }

                    var sheetsSuccess = await _googleSheetsService.SaveRegistrationAsync(queue_info);

                    if (!sheetsSuccess)
                    {

                        ErrorWriter.InsertLogTelegramByUrl(tele_token, tele_group_id, "Lưu ex ko thành công = " + queue_info.ToString());
                        sheetsSuccess = await _googleSheetsService.SaveRegistrationAsync(queue_info);
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
                var SaveRegistration = await _googleSheetsService.SaveRegistrationEX(data);
                if (SaveRegistration == false)
                {
                    SaveRegistration = await _googleSheetsService.SaveRegistrationEX(data);
                }
            }
            catch (Exception ex)
            {
                ErrorWriter.InsertLogTelegramByUrl(tele_token, tele_group_id, "UpdateEx = " + ex.ToString());
            }
        }
    }
}
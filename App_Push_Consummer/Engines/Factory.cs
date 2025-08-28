using App_Push_Consummer.Behaviors;
using App_Push_Consummer.Common;
using App_Push_Consummer.Interfaces;
using App_Push_Consummer.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App_Push_Consummer.Engines
{
    public class Factory : IFactory
    {
        public static string tele_token = ConfigurationManager.AppSettings["tele_token"];
        public static string tele_group_id = ConfigurationManager.AppSettings["tele_group_id"];

        private readonly IGoogleSheetsService _googleSheetsService;

        public Factory(IGoogleSheetsService googleSheetsService)
        {
            _googleSheetsService = googleSheetsService;
        }

        public async void DoSomeRealWork(string data_queue)
        {
            try
            {
                var queue_info = JsonConvert.DeserializeObject<RegistrationRecord>(data_queue);
                if (queue_info != null)
                {
                    var sheetsSuccess = await _googleSheetsService.SaveRegistrationAsync(queue_info);
                    if (!sheetsSuccess)
                    {
                        ErrorWriter.InsertLogTelegramByUrl(tele_token, tele_group_id, "lỗi SaveRegistrationAsync = " + queue_info.ToString());
                    }
                }
                else
                {
                    ErrorWriter.InsertLogTelegramByUrl(tele_token, tele_group_id, "lỗi DeserializeObject = " + data_queue);
                    

                }
            }
            catch (Exception ex)
            {
                ErrorWriter.InsertLogTelegramByUrl(tele_token, tele_group_id, "DoSomeRealWork = " + ex.ToString());
            }
        }
    }
}

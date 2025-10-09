using Entities.ViewModels.Car;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Repositories.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utilities;
using Utilities.Contants;

namespace Web.Cargill.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VehicleInspectionController : ControllerBase
    {
        private readonly IVehicleInspectionRepository _vehicleInspectionRepository;
        public VehicleInspectionController(IVehicleInspectionRepository vehicleInspectionRepository)
        {
            _vehicleInspectionRepository = vehicleInspectionRepository;


        }
        [HttpPost("Insert")]
        public async Task<IActionResult> Insert([FromBody] RegistrationRecord record)
        {
           
            try
            {

                var id = _vehicleInspectionRepository.SaveVehicleInspection(record);
                string url_n8n = "https://n8n.adavigo.com/webhook/text-to-speed";
                record.Bookingid = id;
                var client = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Post, url_n8n);
                request.Content = new StringContent(JsonConvert.SerializeObject(record), null, "application/json");
                var response = await client.SendAsync(request);
               if(response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    LogHelper.InsertLogTelegram("Insert - VehicleInspectionController API: Gửi n8n thành công: " + responseContent);
                }
                else
                {
                    LogHelper.InsertLogTelegram("Insert - VehicleInspectionController API: Gửi n8n thất bại: " + response.StatusCode);
                }
                return Ok(new
                { 
                    status= (int)ResponseType.SUCCESS,
                    message = "Upload audio thành công",
                    data = id
                });
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("Insert - VehicleInspectionController API: " + ex);
                return Ok(new
                {
                    status = (int)ResponseType.ERROR,
                    message = "đã xẩy ra lỗi vui lòng liên hệ IT",
                   
                });
            }
        }
    }
}

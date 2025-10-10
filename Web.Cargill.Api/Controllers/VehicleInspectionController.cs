using B2B.Utilities.Common;
using Entities.ViewModels.Car;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Repositories.IRepositories;
using Utilities.Contants;

namespace Web.Cargill.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VehicleInspectionController : ControllerBase
    {
        private readonly IVehicleInspectionRepository _vehicleInspectionRepository;
        public VehicleInspectionController(IVehicleInspectionRepository vehicleInspectionRepository)
        {
            _vehicleInspectionRepository = vehicleInspectionRepository;


        }
        [HttpPost("Insert")]
        public async Task<IActionResult> Insert([FromBody] RegistrationRecord request)
        {
           
            try
            {
                

                var id = _vehicleInspectionRepository.SaveVehicleInspection(request);
                LogHelper.InsertLogTelegram("Insert - VehicleInspectionController API: Gửi n8n thất bại:  Id" + id);
                //string url_n8n = "https://n8n.adavigo.com/webhook/text-to-speed";
                //request.Bookingid = id;
                //var client = new HttpClient();
                //var request_n8n = new HttpRequestMessage(HttpMethod.Post, url_n8n);
                //request_n8n.Content = new StringContent(JsonConvert.SerializeObject(request), null, "application/json");
                //var response = await client.SendAsync(request_n8n);
                //if (response.IsSuccessStatusCode)
                //{
                //    var responseContent = await response.Content.ReadAsStringAsync();
                   
                //}
                //else
                //{
                //    LogHelper.InsertLogTelegram("Insert - VehicleInspectionController API: Gửi n8n thất bại:  Id" + id);
                //}
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

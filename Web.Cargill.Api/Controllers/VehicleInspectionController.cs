using Entities.ViewModels.Car;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
        public async Task<IActionResult> Insert(RegistrationRecord record)
        {
           
            try
            {

                var id = _vehicleInspectionRepository.SaveVehicleInspection(record);
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

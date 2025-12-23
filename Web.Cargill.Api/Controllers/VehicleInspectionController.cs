using B2B.Utilities.Common;
using Entities.ViewModels.Car;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Repositories.IRepositories;
using Repositories.Repositories;
using Utilities.Contants;
using Web.Cargill.Api.Services;

namespace Web.Cargill.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VehicleInspectionController : ControllerBase
    {
        private readonly IVehicleInspectionRepository _vehicleInspectionRepository;
        private readonly RedisConn redisService;
        private readonly IConfiguration _configuration;
        private readonly WorkQueueClient _workQueueClient;
        private readonly IAllCodeRepository _allCodeRepository;

        public VehicleInspectionController(IVehicleInspectionRepository vehicleInspectionRepository, IConfiguration configuration, IAllCodeRepository allCodeRepository)
        {
            _vehicleInspectionRepository = vehicleInspectionRepository;
            redisService = new RedisConn(configuration);
            redisService.Connect();
            _configuration = configuration;
            _workQueueClient = new WorkQueueClient(configuration);
            _allCodeRepository = allCodeRepository;

        }
        [HttpPost("Insert")]
        public async Task<IActionResult> Insert([FromBody] RegistrationRecord request)
        {

            try
            {
                var audio = await _vehicleInspectionRepository.GetAudioPathByVehicleNumberAPI(request.PlateNumber, request.LocationType);
                if (!string.IsNullOrEmpty(audio))
                {
                    request.AudioPath = audio;
                    LogHelper.InsertLogTelegram("sql:" + request.PlateNumber);
                }
                var id = _vehicleInspectionRepository.SaveVehicleInspectionAPI(request);
                if (id > 0 && (request.AudioPath == null || request.AudioPath == ""))
                {
                    request.Id = id;
                    request.Bookingid = id;
                    request.text_voice = "Mời biển số xe " + request.PlateNumber + " vào cân";
                    switch(request.LocationType)
                    {
                        case 0:
                        case 1:
                            {
                                await redisService.PublishAsync("Add_ReceiveRegistration" + request.LocationType, request);
                                LogHelper.InsertLogTelegram("Queue :" + request.PlateNumber);
                                var Queue = _workQueueClient.SyncQueue(request);
                                if (!Queue)
                                {
                                    Queue = _workQueueClient.SyncQueue(request);
                                }
                            }
                            break;
                        case 2:
                            {
                                await redisService.PublishAsync("Add_ReceiveRegistration" + request.LocationType, request);
                                LogHelper.InsertLogTelegram("Queue LA:" + request.PlateNumber);
                                var Queue = _workQueueClient.SyncQueue(request);
                                if (!Queue)
                                {
                                    Queue = _workQueueClient.SyncQueue(request);
                                }
                            }
                            break;
                       
                        default:
                            break;
                    }
                  
               
                   
                    return Ok(new
                    {
                        status = (int)ResponseType.SUCCESS,
                        message = "Upload audio thành công",
                        data = id
                    });
                }
                if (id > 0)
                {
                    return Ok(new
                    {
                        status = (int)ResponseType.SUCCESS,
                        message = "Thêm mới thành công",
                        data = id
                    });
                }
                else
                {
                    LogHelper.InsertLogTelegram("Insert - lỗi :" + request.PlateNumber);
                    return Ok(new
                    {
                        status = (int)ResponseType.ERROR,
                        message = "Thêm mới lỗi",

                    });
                }
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
        [HttpGet("report/search-by-date")]
        public async Task<IActionResult> SearchByDate(
            [FromQuery(Name = "date")] DateTime date)
        {
            try
            {
                var fromDate = date.Date;

                var request = new CartoFactorySearchModel
                {
                    RegisterDateOnline = fromDate
                };

                var data = await _vehicleInspectionRepository
                    .SearchVehicleInspection(request);

                var result = data?.Select(x => new CartoFactoryResponseDto
                {
                    Id = x.Id,
                    RecordNumber = x.RecordNumber,
                    CustomerName = x.CustomerName,
                    VehicleNumber = x.VehicleNumber,
                    RegisterDateOnline = x.RegisterDateOnline,
                    DriverName = x.DriverName,
                    LicenseNumber = x.LicenseNumber,
                    PhoneNumber = x.PhoneNumber,
                    VehicleLoad = x.VehicleLoad,
                    VehicleStatus = x.VehicleStatus,
                    VehicleStatusName = x.VehicleStatusName,
                    AudioPath = x.AudioPath
                }).ToList() ?? new List<CartoFactoryResponseDto>();

                return Ok(new
                {
                    status = 1,
                    message = "Lấy dữ liệu thành công",
                    data = result
                });
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("SearchByDate error: " + ex);
                return Ok(new
                {
                    status = 0,
                    message = "Đã xảy ra lỗi, vui lòng liên hệ IT"
                });
            }
        }


        // =========================
        // 2️⃣ REPORT SUMMARY (FROM - TO)
        // =========================
        [HttpGet("report/summary")]
        public async Task<IActionResult> GetSummary(
            [FromQuery(Name = "from_date")] DateTime fromDate,
            [FromQuery(Name = "to_date")] DateTime toDate)
        {
            try
            {
                if (fromDate > toDate)
                {
                    return Ok(new
                    {
                        status = 0,
                        message = "from_date không được lớn hơn to_date"
                    });
                }

                var data = await _vehicleInspectionRepository
                    .CountTotalVehicleInspectionSynthetic(fromDate.Date, toDate.Date);

                if (data == null)
                {
                    return Ok(new
                    {
                        status = 0,
                        message = "Không có dữ liệu",
                        data = (object)null
                    });
                }

                return Ok(new
                {
                    status = 1,
                    message = "Lấy dữ liệu thành công",
                    data
                });
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetSummary error: " + ex);
                return Ok(new
                {
                    status = 0,
                    message = "Đã xảy ra lỗi, vui lòng liên hệ IT"
                });
            }
        }

        // =========================
        // 3️⃣ REPORT BY WEIGHT GROUP (1 DAY)
        // =========================
        [HttpGet("report/by-weight-group")]
        public async Task<IActionResult> GetByWeightGroup(
            [FromQuery(Name = "date")] DateTime date)
        {
            try
            {
                var data = await _vehicleInspectionRepository
                    .GetTotalWeightByWeightGroup(date.Date);

                if (data == null || !data.Any())
                {
                    return Ok(new
                    {
                        status = 0,
                        message = "Không có dữ liệu",
                        data = new object[] { }
                    });
                }

                return Ok(new
                {
                    status = 1,
                    message = "Lấy dữ liệu thành công",
                    data
                });
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetByWeightGroup error: " + ex);
                return Ok(new
                {
                    status = 0,
                    message = "Đã xảy ra lỗi, vui lòng liên hệ IT"
                });
            }
        }

        // =========================
        // 4️⃣ REPORT BY TROUGH (1 DAY)
        // =========================
        [HttpGet("report/by-trough")]
        public async Task<IActionResult> GetByTrough(
            [FromQuery(Name = "date")] DateTime date)
        {
            try
            {
                var data = await _vehicleInspectionRepository
                    .GetTotalWeightByTroughType(date.Date);

                if (data == null || !data.Any())
                {
                    return Ok(new
                    {
                        status = 0,
                        message = "Không có dữ liệu",
                        data = new object[] { }
                    });
                }

                return Ok(new
                {
                    status = 1,
                    message = "Lấy dữ liệu thành công",
                    data
                });
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetByTrough error: " + ex);
                return Ok(new
                {
                    status = 0,
                    message = "Đã xảy ra lỗi, vui lòng liên hệ IT"
                });
            }
        }
        [HttpGet("get-time-countdown")]
        public async Task<IActionResult> GetTimeCountdown()
        {
            try
            {
                var TIME_RESET = await _allCodeRepository.GetListSortByName(AllCodeType.TIME_RESET);
                if (TIME_RESET == null || TIME_RESET.Count == 0)
                {
                    return Ok(new
                    {
                        status = (int)ResponseType.ERROR,
                        message = "Chưa cấu hình thời gian đặt lại",
                    });
                }
                return Ok(new
                {
                    status = (int)ResponseType.SUCCESS,
                    message = "Upload audio thành công",
                    data = TIME_RESET != null && TIME_RESET.Count > 0 && TIME_RESET[0].UpdateTime.HasValue
                            ? TIME_RESET[0].UpdateTime.Value.ToString("dd/MM/yyyy HH:mm:ss") : ""
                });
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetTimeCountdown - VehicleInspectionController API: " + ex);
                return Ok(new
                {
                    status = (int)ResponseType.ERROR,
                    message = "đã xẩy ra lỗi vui lòng liên hệ IT",
                });
            }
        }
        [HttpPost("update-by-plateNumber")]
        public async Task<IActionResult> UpdateByPlateNumber([FromBody] CamModel request)
        {
            try
            {
                var update = await _vehicleInspectionRepository.UpdateVehicleInspectionByVehicleNumber(request.bien_so);
                if (update > 0)
                {
                    var detail = await _vehicleInspectionRepository.GetDetailtVehicleInspection(update);
                    await redisService.Publish_CamAsync("ListCartoFactory_Cam", detail);
                    return Ok(new
                    {
                        status = (int)ResponseType.SUCCESS,
                        message = "Cập nhật thành công",
                    });

                }
                else
                {
                    return Ok(new
                    {
                        status = (int)ResponseType.ERROR,
                        message = "Cập nhật thất bại",
                    });
                }


            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("UpdateByPlateNumber - VehicleInspectionController API: " + ex);
                return Ok(new
                {
                    status = (int)ResponseType.ERROR,
                    message = "đã xẩy ra lỗi vui lòng liên hệ IT",
                });
            }
        }
    }
}

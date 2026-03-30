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
                var TIME_RESET = await _allCodeRepository.GetListSortByName_LA(AllCodeType.TIME_RESET,request.LocationType);
                
                string cache_name = "CARGLL_LongAn";
                var data_list = new List<RegistrationRecord>();
                var data = await redisService.GetAsync(cache_name, Convert.ToInt32(_configuration["Redis:Database:db_common_longan"]));
                if (data != null && data.Trim() != "")
                {
                    data_list = JsonConvert.DeserializeObject<List<RegistrationRecord>>(data);
                    if (data_list != null && data_list.Count> 0)
                    {
                        foreach (var item in data_list)
                        {
                            var audio_longan = await _vehicleInspectionRepository.GetAudioPathByVehicleNumberAPI(item.PlateNumber, item.LocationType);
                            if (!string.IsNullOrEmpty(audio_longan))
                            {
                                item.AudioPath = audio_longan;
                            }
                           var RegisterDateOnline= Utilities.DateUtil.StringToDateTime(item.RegistrationTime.ToString("dd/MM/yyyy HH:mm:ss"));
                            var check = _vehicleInspectionRepository.checkVehicleInspectionbyRegisterDateOnline(RegisterDateOnline, item.PlateNumber);
                            if (check != null) {
                                continue;
                            }
                            var Save_id = _vehicleInspectionRepository.SaveVehicleInspectionAPI(item);
                            
                            if (Save_id > 0 && (item.AudioPath == null || item.AudioPath == ""))
                            {
                                item.Id = Save_id;
                                item.Bookingid = Save_id;
                                item.text_voice = "Mời biển số xe " + item.PlateNumber + " vào cân";
                                var Queue = _workQueueClient.SyncQueue(item);
                                if (!Queue)
                                {
                                    Queue = _workQueueClient.SyncQueue(item);
                                }
                                await redisService.PublishAsync("Add_ReceiveRegistration_LongAn", item);
                                LogHelper.InsertLogTelegram("PublishAsync LA:" + item.PlateNumber + " -id=" + item.Id);


                            }
                           else
                            {
                                item.Id = Save_id;
                                item.Bookingid = Save_id;
                                await redisService.PublishAsync("Add_ReceiveRegistration_LongAn", item);
                                LogHelper.InsertLogTelegram("PublishAsync LA:" + item.PlateNumber + " -id=" + item.Id);

                            }
                            
                        }
                    }
                     redisService.clear(cache_name, Convert.ToInt32(_configuration["Redis:Database:db_common"]));
                }
                var audio = await _vehicleInspectionRepository.GetAudioPathByVehicleNumberAPI(request.PlateNumber, request.LocationType);
                if (!string.IsNullOrEmpty(audio))
                {
                    request.AudioPath = audio;
                    LogHelper.InsertLogTelegram("sql:" + request.PlateNumber);
                }
                LogHelper.InsertLogTelegram("sql:" + request.PlateNumber+":"+ request.LocationType);
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
                                var Queue = _workQueueClient.SyncQueue(request);
                                if (!Queue)
                                {
                                    Queue = _workQueueClient.SyncQueue(request);
                                }
                                await redisService.PublishAsync("Add_ReceiveRegistration" + request.LocationType, request);
                                LogHelper.InsertLogTelegram("PublishAsync Queue :" + request.PlateNumber + " -id=" + request.Id);
                            }
                            break;
                        case 2:
                            {
                                var Queue = _workQueueClient.SyncQueue(request);
                                if (!Queue)
                                {
                                    Queue = _workQueueClient.SyncQueue(request);
                                }
                                if(DateTime.Now.TimeOfDay <= ((DateTime)TIME_RESET[0].UpdateTime).TimeOfDay)
                                {
                                    await redisService.PublishAsync("Add_ReceiveRegistration_LongAn", request);

                                }
                                else
                                {
                                    await redisService.PublishAsync("Add_ReceiveRegistration_LongAn_DK", request);
                                }
                                LogHelper.InsertLogTelegram("PublishAsync Queue LA:" + request.PlateNumber + " -id=" + request.Id);
                            }
                            break;
                       case 3:{
                                var Queue = _workQueueClient.SyncQueue(request);
                                if (!Queue)
                                {
                                    Queue = _workQueueClient.SyncQueue(request);
                                }
                                if (DateTime.Now.TimeOfDay <= ((DateTime)TIME_RESET[0].UpdateTime).TimeOfDay)
                                {
                                    await redisService.PublishAsync("Add_ReceiveRegistration_BinhDinh", request);

                                }
                                else
                                {
                                    await redisService.PublishAsync("Add_ReceiveRegistration_BinhDinh_DK", request);
                                }
                                LogHelper.InsertLogTelegram("PublishAsync Queue BinhDinh:" + request.PlateNumber + " -id=" + request.Id);
                       }break;
                       case 4:{
                                var Queue = _workQueueClient.SyncQueue(request);
                                if (!Queue)
                                {
                                    Queue = _workQueueClient.SyncQueue(request);
                                }
                                if (DateTime.Now.TimeOfDay <= ((DateTime)TIME_RESET[0].UpdateTime).TimeOfDay)
                                {
                                    await redisService.PublishAsync("Add_ReceiveRegistration_BinhDuong", request);

                                }
                                else
                                {
                                    await redisService.PublishAsync("Add_ReceiveRegistration_BinhDuong_DK", request);
                                }
                                LogHelper.InsertLogTelegram("PublishAsync Queue BinhDuong:" + request.PlateNumber + " -id=" + request.Id);
                       }break;
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
                    request.Id = id;
                    request.Bookingid = id;
                    switch (request.LocationType)
                    {
                        case 0:
                        case 1:
                            {
                                await redisService.PublishAsync("Add_ReceiveRegistration" + request.LocationType, request);
                                LogHelper.InsertLogTelegram("PublishAsync :" + request.PlateNumber +" -id="+ request.Id);
                               
                            }
                            break;
                        case 2:
                            {
                                if (DateTime.Now.TimeOfDay <= ((DateTime)TIME_RESET[0].UpdateTime).TimeOfDay)
                                {
                                    await redisService.PublishAsync("Add_ReceiveRegistration_LongAn", request);

                                }
                                else
                                {
                                    await redisService.PublishAsync("Add_ReceiveRegistration_LongAn_DK", request);

                                }
                                LogHelper.InsertLogTelegram("PublishAsync LongAn:" + request.PlateNumber + " -id=" + request.Id);
                               
                            }
                            break;
                        case 3:
                            {
                                if (DateTime.Now.TimeOfDay <= ((DateTime)TIME_RESET[0].UpdateTime).TimeOfDay)
                                {
                                    await redisService.PublishAsync("Add_ReceiveRegistration_BinhDinh", request);

                                }
                                else
                                {
                                    await redisService.PublishAsync("Add_ReceiveRegistration_BinhDinh_DK", request);

                                }
                                LogHelper.InsertLogTelegram("PublishAsync BinhDinh:" + request.PlateNumber + " -id=" + request.Id);
                            }
                            break;
                        case 4:
                            {
                                if (DateTime.Now.TimeOfDay <= ((DateTime)TIME_RESET[0].UpdateTime).TimeOfDay)
                                {
                                    await redisService.PublishAsync("Add_ReceiveRegistration_BinhDuong", request);

                                }
                                else
                                {
                                    await redisService.PublishAsync("Add_ReceiveRegistration_BinhDuong_DK", request);

                                }
                                LogHelper.InsertLogTelegram("PublishAsync BinhDuong:" + request.PlateNumber + " -id=" + request.Id);
                            }
                            break;
                        default:
                            break;
                    }

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
                    RegistrationTime = fromDate
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
        public async Task<IActionResult> GetTimeCountdown(int LocationType=2)
        {
            try
            {
                var TIME_RESET = await _allCodeRepository.GetListSortByName_LA(AllCodeType.TIME_RESET, LocationType);
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

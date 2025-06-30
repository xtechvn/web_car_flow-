﻿using Microsoft.AspNetCore.Mvc;
using XTECH_FRONTEND.IRepositories;
using XTECH_FRONTEND.Model;


namespace XTECH_FRONTEND.Controllers.CarRegistration
{
    [ApiController]
    [Route("api/[controller]")]
    public class CarRegistrationController : Controller
    {
        private readonly IValidationService _validationService;
        private readonly IGoogleSheetsService _googleSheetsService;
        private readonly IGoogleFormsService _googleFormsService;
        private readonly IZaloService _zaloService;
        private readonly ILogger<CarRegistrationController> _logger;

        public CarRegistrationController(
            IValidationService validationService,
            IGoogleSheetsService googleSheetsService,
            IGoogleFormsService googleFormsService,
            IZaloService zaloService,
            ILogger<CarRegistrationController> logger)
        {
            _validationService = validationService;
            _googleSheetsService = googleSheetsService;
            _googleFormsService = googleFormsService;
            _zaloService = zaloService;
            _logger = logger;
        }
        [HttpPost("register")]
        public async Task<ActionResult<CarRegistrationResponse>> RegisterCar([FromBody] CarRegistrationRequest request)
        {
            try
            {
                _logger.LogInformation($"Car registration request received: {request.PhoneNumber} - {request.PlateNumber}");

                // Step 1: Validate input data
                var validationResult = _validationService.ValidateCarRegistration(request);
                if (!validationResult.IsValid)
                {
                    return BadRequest(new CarRegistrationResponse
                    {
                        Success = false,
                        Message = string.Join(", ", validationResult.Errors)
                    });
                }

                // Step 2: Check time restriction (15 minutes rule)
                var timeRestriction = _validationService.CheckTimeRestriction(request.PlateNumber);
                if (!timeRestriction.CanSubmit)
                {
                    return BadRequest(new CarRegistrationResponse
                    {
                        Success = false,
                        Message = $"Vui lòng đợi {timeRestriction.RemainingMinutes} phút trước khi gửi lại",
                        RemainingTimeMinutes = timeRestriction.RemainingMinutes
                    });
                }

                // Step 3: Get current daily queue count
                var dailyCount = await _googleSheetsService.GetDailyQueueCountAsync();
                var queueNumber = dailyCount + 1;

                // Step 4: Create registration record with initial Zalo status
                var registrationRecord = new RegistrationRecord
                {
                    PhoneNumber = request.PhoneNumber,
                    PlateNumber = request.PlateNumber.ToUpper(),
                    Name = request.Name.ToUpper(),
                    Referee = request.Referee.ToUpper(),
                    GPLX = request.GPLX.ToUpper(),
                    QueueNumber = queueNumber,
                    RegistrationTime = DateTime.Now,
                    ZaloStatus = "Đang xử lý..."
                };

                // Step 5: Submit to Google Form
                var formSubmissionSuccess = await _googleFormsService.SubmitToGoogleFormAsync(registrationRecord);
                if (!formSubmissionSuccess)
                {
                    _logger.LogWarning("Google Form submission failed, but continuing...");
                }

                // Step 6: Send Zalo notification and get status
                var (zaloSuccess, zaloStatus) = await _zaloService.SendRegistrationNotificationAsync(registrationRecord);

                // Update registration record with Zalo status
                registrationRecord.ZaloStatus = zaloStatus;

                // Step 7: Save to Google Sheets with Zalo status
                var sheetsSuccess = await _googleSheetsService.SaveRegistrationAsync(registrationRecord);
                if (!sheetsSuccess)
                {
                    return StatusCode(500, new CarRegistrationResponse
                    {
                        Success = false,
                        Message = "Lỗi hệ thống, vui lòng thử lại sau"
                    });
                }

                // Step 8: Update last submission time
                await _googleSheetsService.UpdateLastSubmissionTimeAsync(request.PlateNumber, DateTime.Now);

                // Return success response
                return Ok(new CarRegistrationResponse
                {
                    Success = true,
                    Message = "Đăng ký thành công!",
                    QueueNumber = queueNumber,
                    RegistrationTime = registrationRecord.RegistrationTime,
                    PlateNumber = registrationRecord.PlateNumber,
                    PhoneNumber = registrationRecord.PhoneNumber,
                    ZaloStatus = zaloStatus
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing car registration");
                return StatusCode(500, new CarRegistrationResponse
                {
                    Success = false,
                    Message = "Lỗi hệ thống, vui lòng thử lại sau"
                });
            }
        }

        [HttpGet("check-restriction/{plateNumber}")]
        public ActionResult<TimeRestrictionResult> CheckTimeRestriction(string PlateNumber)
        {
            try
            {
                var result = _validationService.CheckTimeRestriction(PlateNumber);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error checking time restriction for {PlateNumber}");
                return StatusCode(500, "Lỗi hệ thống");
            }
        }

        [HttpGet("queue-status")]
        public async Task<ActionResult<object>> GetQueueStatus()
        {
            try
            {
                var dailyCount = await _googleSheetsService.GetDailyQueueCountAsync();
                return Ok(new
                {
                    CurrentQueueNumber = dailyCount,
                    NextQueueNumber = dailyCount + 1,
                    Date = DateTime.Today.ToString("yyyy-MM-dd")
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting queue status");
                return StatusCode(500, "Lỗi hệ thống");
            }
        }
        [HttpGet("check-zalo-user/{phoneNumber}")]
        public async Task<ActionResult> CheckZaloUser(string phoneNumber)
        {
            try
            {
                var userDetail = await _zaloService.GetUserDetailByPhoneAsync(phoneNumber);

                if (userDetail == null)
                {
                    return Ok(new
                    {
                        exists = false,
                        message = "Số điện thoại này chưa được Approve Zalo OA"
                    });
                }

                return Ok(new
                {
                    exists = true,
                    userId = userDetail.user_id,
                    displayName = userDetail.display_name,
                    isFollower = userDetail.user_is_follower,
                    lastInteraction = userDetail.user_last_interaction_date,
                    avatar = userDetail.Avatar,
                    status = userDetail.user_is_follower ? "Có thể gửi tin nhắn" : "Chưa follow OA"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error checking Zalo user for {phoneNumber}");
                return StatusCode(500, new { message = "Lỗi hệ thống" });
            }
        }
    }
}

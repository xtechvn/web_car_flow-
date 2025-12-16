using Microsoft.AspNetCore.Mvc;
using Repositories.IRepositories;
using Repositories.Repositories;
using System;
using Utilities;
using Web.Cargill.Api.Model;
using Web.Cargill.Api.Services;

namespace Web.Cargill.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly IWebHostEnvironment _env;
        private readonly IConfiguration _config;
        private readonly IVehicleInspectionRepository _vehicleInspectionRepository;
        private readonly MailService _mailService;

        public ReportController(AppDbContext db, IWebHostEnvironment env, IConfiguration config, IVehicleInspectionRepository vehicleInspectionRepository )
        {
            _db = db;
            _env = env;
            _config = config;
            _vehicleInspectionRepository = vehicleInspectionRepository;
            _mailService = new MailService(_config);
        }

        [HttpPost("send-daily-mail")]
        public async Task<IActionResult> SendDailyMail()
        {
            try
            {
                // Ngày báo cáo (chỉ cần ngày)
                var reportDate = DateUtil.Now.Date;

                // 1️⃣ Tổng hợp chỉ tiêu chính
                var summary = await _vehicleInspectionRepository
                    .CountTotalVehicleInspectionSynthetic(reportDate, reportDate);

                if (summary == null)
                {
                    return Ok(new { status = 0, message = "Không có dữ liệu" });
                }

                // 2️⃣ Theo nhóm xe
                var byWeightGroup = await _vehicleInspectionRepository
                    .GetTotalWeightByWeightGroup(reportDate);

                // 3️⃣ Theo máng
                var byTrough = await _vehicleInspectionRepository
                    .GetTotalWeightByTroughType(reportDate);

                // 4️⃣ Gửi mail
                var sent = await _mailService.SendDailyVehicleReportMail(
                    summary,
                    byWeightGroup,
                    byTrough,
                    reportDate
                );

                return Ok(new
                {
                    status = sent ? 1 : 0,
                    message = sent ? "Gửi mail thành công" : "Gửi mail thất bại"
                });
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram(
                    $"SendDailyMail API ERROR\n" +
                    $"Message: {ex.Message}\n" +
                    $"StackTrace: {ex.StackTrace}"
                );

                return StatusCode(500, new
                {
                    status = 0,
                    message = "SendDailyMail failed",
                    error = ex.Message,
                    detail = ex.InnerException?.Message
                });
            }

        }




    }
}

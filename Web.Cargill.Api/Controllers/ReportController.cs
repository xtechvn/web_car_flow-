using Microsoft.AspNetCore.Mvc;
using Repositories.IRepositories;
using Repositories.Repositories;
using System;
using Utilities;
using Web.Cargill.Api.Model;
using Web.Cargill.Api.Services;

namespace Web.Cargill.Api.Controllers
{
    public class ReportController : Controller
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

        [HttpPost("report/send-daily-mail")]
        public async Task<IActionResult> SendDailyMail()
        {
            try
            {
                var vnTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
                var vnNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vnTimeZone);
                var reportDate = vnNow.Date;

                var summary = await _vehicleInspectionRepository
                    .CountTotalVehicleInspectionSynthetic(reportDate, reportDate);

                var byWeightGroup = await _vehicleInspectionRepository
                    .GetTotalWeightByWeightGroup(reportDate);

                var byTrough = await _vehicleInspectionRepository
                    .GetTotalWeightByTroughType(reportDate);

                if (summary == null)
                    return Ok(new { status = 0, message = "Không có dữ liệu" });

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
                LogHelper.InsertLogTelegram("SendDailyMail API: " + ex);
                return Ok(new { status = 0, message = "Lỗi hệ thống" });
            }
        }




    }
}

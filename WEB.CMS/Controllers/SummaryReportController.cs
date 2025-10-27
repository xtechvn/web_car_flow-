using Entities.ViewModels.Car;
using Microsoft.AspNetCore.Mvc;
using Nest;
using Repositories.IRepositories;
using Repositories.Repositories;
using Utilities;
using Utilities.Contants;

namespace WEB.CMS.Controllers
{
    public class SummaryReportController : Controller
    {
        private readonly IVehicleInspectionRepository _vehicleInspectionRepository;

        private readonly IConfiguration _configuration;
        public SummaryReportController(IVehicleInspectionRepository vehicleInspectionRepository, IConfiguration configuration)
        {
            _vehicleInspectionRepository = vehicleInspectionRepository;
            _configuration = configuration;

        }
        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> DailyStatistics(string date)
        {
            try
            {
                var date_time = date!=null && date !=""? DateUtil.Parse(date):null;
                var data = await _vehicleInspectionRepository.GetListVehicleInspectionSynthetic(date_time);
                var Total = await _vehicleInspectionRepository.CountTotalVehicleInspectionSynthetic(date_time);
                ViewBag.TotalData = Total;
                if(data != null && data.Count > 0)
                {
                  var  SumTime= data.Sum(s=> s.VehicleWeighingTimeComplete.HasValue && s.VehicleWeighingTimeComeIn.HasValue ? (s.VehicleWeighingTimeComplete.Value - s.VehicleWeighingTimeComeIn.Value).TotalMinutes : 0);
                 
                    var tb= SumTime / Total.TotalCarCompleted;
                    var SumTime_ngay= (data.Max(s=>s.VehicleWeighingTimeComplete).Value - data.Min(s =>s.VehicleWeighingTimeComeIn).Value).TotalMinutes;
                    ViewBag.AverageWeighingTime = tb;
                    ViewBag.SumTime_ngay = SumTime_ngay;
                }
                return PartialView(data);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("DailyStatistics - SummaryReportController: " + ex);
            }
            return PartialView();
        }
    }
}

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
                var date_time = date!=null && date !=""? DateUtil.StringToDate(date):null;
                var data = await _vehicleInspectionRepository.GetListVehicleInspectionSynthetic(date_time);
                var Total = await _vehicleInspectionRepository.CountTotalVehicleInspectionSynthetic(date_time);
                ViewBag.TotalData = Total;
                
                return PartialView(data);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("DailyStatistics - SummaryReportController: " + ex);
            }
            return PartialView();
        }
        public async Task<IActionResult> GetTotalWeightByHour(string date)
        {
            try
            {
                var date_time = date!=null && date !=""? DateUtil.StringToDate(date):null;
            
                var data = await _vehicleInspectionRepository.GetTotalWeightByHour(date_time);
                var datamodel=new TotalWeightByHourViewModel();
                datamodel.CompletionHour = data.Select(x => x.CompletionHour).ToArray();
                datamodel.TotalWeightInHour = data.Select(x => x.TotalWeightInHour).ToArray();

                return Ok(new
                {
                    isSuccess = true,
                    data = datamodel
                });
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetTotalWeightByHour - SummaryReportController: " + ex);
            }
            return Ok(new
            {
                isSuccess = false,
               
            });
        }
       public async Task<IActionResult> GetProductivityStatistics(string date)
        {
            try
            {
                var date_time = date != null && date != "" ? DateUtil.StringToDate(date) : null;
                var data = await _vehicleInspectionRepository.CountTotalVehicleInspectionSynthetic(date_time);
             
                return PartialView(data);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetProductivityStatistics - SummaryReportController: " + ex);
            }
            return PartialView();
        }
    }
}

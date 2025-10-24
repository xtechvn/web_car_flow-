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
        public async Task<IActionResult> DailyStatistics(CartoFactorySearchModel SearchModel)
        {
            try
            {
       
                var data = await _vehicleInspectionRepository.GetListVehicleInspectionSynthetic(SearchModel);
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

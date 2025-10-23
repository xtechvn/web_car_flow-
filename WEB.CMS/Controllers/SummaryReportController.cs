using Microsoft.AspNetCore.Mvc;
using Repositories.IRepositories;

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
        public IActionResult DailyStatistics(string datetime)
        {
            return View();
        }
    }
}

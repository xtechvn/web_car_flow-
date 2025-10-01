using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Utilities;
using WEB.CMS.Customize;

namespace WEB.CMS.Controllers
{
    [CustomAuthorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            try
            {
     
  
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                LogHelper.InsertLogTelegram("Index - HomeController: " + ex);
            }
            return View();
        }

        public IActionResult DataMonitor()
        {
            return RedirectToAction("Index", "Error");
        }

        public IActionResult ExecuteQuery(string dataQuery)
        {

            return RedirectToAction("Index", "Error");
        }

        public IActionResult Privacy()
        {
            return View();
        }
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Error()
        {
            ViewBag.UserName = "";
            return View();
        }
    }
    
}

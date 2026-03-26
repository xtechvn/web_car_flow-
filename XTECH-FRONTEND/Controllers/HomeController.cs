using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Text.Encodings.Web;
using System.Xml.Linq;
using XTECH_FRONTEND.IRepositories;
using XTECH_FRONTEND.Utilities;

namespace XTECH_FRONTEND.Controllers
{
    public class HomeController : Controller
    {
        private readonly IMongoService _mongoService;
        public HomeController(IMongoService mongoService)
        {
            _mongoService = mongoService;
        }
        // GET: /home/
        public IActionResult Index()
        {
            var vnTime = TimeZoneInfo.ConvertTimeFromUtc(
                DateTime.UtcNow,
                TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time")
            );
            var today = vnTime.Date;

            var lockStart = today.AddHours(17).AddMinutes(55); // 17:55
            var lockEnd = today.AddHours(18); // 18:00
            ViewBag.serverTime = vnTime;
            ViewBag.lockStart = lockStart;
            ViewBag.lockEnd = lockEnd;
            return View();           
        }
        public IActionResult ListData()
        {
            var data = _mongoService.GetList();
            ViewBag.Data = data;
            return View();
        }
        //home/welcome?a=hello&b=2
        public string Welcome(string a,int  b=1)
        {
            return HtmlEncoder.Default.Encode($"Hello {a}, NumTimes is: {b}");
        }       

    }
}

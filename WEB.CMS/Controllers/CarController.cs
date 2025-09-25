using Entities.Models;
using Entities.ViewModels.Car;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;
using Repositories.IRepositories;
using Repositories.Repositories;
using System.Threading.Tasks;
using Utilities;
using Utilities.Contants;
using WEB.CMS.Customize;

namespace WEB.CMS.Controllers
{
    [CustomAuthorize]
    public class CarController : Controller
    {
        private readonly IVehicleInspectionRepository _vehicleInspectionRepository;
        private readonly IAllCodeRepository _allCodeRepository;
      
        public CarController(IVehicleInspectionRepository vehicleInspectionRepository, IAllCodeRepository allCodeRepository)
        {
            _vehicleInspectionRepository = vehicleInspectionRepository;
            _allCodeRepository = allCodeRepository;
        }
        public IActionResult CartoFactory()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("CartoFactory - CarController: " + ex);
            }
            return View();
        } 
        public async Task<IActionResult> ListCartoFactory(CartoFactorySearchModel SearchModel)
        {
            try
            {
                var data = await _vehicleInspectionRepository.GetListCartoFactory(SearchModel);
                return PartialView(data);
            }
            catch(Exception ex)
            {
                LogHelper.InsertLogTelegram("ListCartoFactory - CarController: " + ex);
            }
            return PartialView();
        }
        public async Task<IActionResult> OpenPopup(int id, int type,int status)
        {
            try
            {
                ViewBag.Id = id;
                ViewBag.StatusCar = status;
                var data = new List<AllCode>();
                switch (type)
                {
                    case 1:
                        data = await _allCodeRepository.GetListSortByName(AllCodeType.VEHICLE_STATUS);
                        break;
                    case 2:
                        data = await _allCodeRepository.GetListSortByName(AllCodeType.LOAD_TYPE);
                        break;
                    case 3:
                        data = await _allCodeRepository.GetListSortByName(AllCodeType.VEHICLEWEIGHING_TYPE);
                        break;
                    case 4:
                        data = await _allCodeRepository.GetListSortByName(AllCodeType.TROUGH_TYPE);
                        break;
                    case 5:
                        data = await _allCodeRepository.GetListSortByName(AllCodeType.VEHICLETROUG_HWEIGHT);
                        break;
                    case 6:
                        data = await _allCodeRepository.GetListSortByName(AllCodeType.VEHICLETROUGH_STATUS);
                        break;
                    case 7:
                        data = await _allCodeRepository.GetListSortByName(AllCodeType.VEHICLEWEIGHINGSTATUS);
                        break;
                }
                ViewBag.Status = data;

            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("OpenPopup - CarController: " + ex);
            }
            return PartialView();
        }
        public IActionResult ProcessingIsLoading()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("CartoFactory - CarController: " + ex);
            }
            return View();
        }  
        public IActionResult CallTheScale()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("CartoFactory - CarController: " + ex);
            }
            return View();
        }  
        public IActionResult WeighedInput()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("CartoFactory - CarController: " + ex);
            }
            return View();
        }
    }
}

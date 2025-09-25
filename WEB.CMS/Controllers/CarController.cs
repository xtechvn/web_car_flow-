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
                var AllCode = await _allCodeRepository.GetListSortByName(AllCodeType.VEHICLE_STATUS);
                ViewBag.AllCode = AllCode;
                var data = await _vehicleInspectionRepository.GetListCartoFactory(SearchModel);
                return PartialView(data);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("ListCartoFactory - CarController: " + ex);
            }
            return PartialView();
        }
        public async Task<IActionResult> OpenPopup(int id, int type)
        {
            try
            {
                ViewBag.Id = id;
                ViewBag.StatusCar = 0;
                var data = new List<AllCode>();
                var detail = await _vehicleInspectionRepository.GetDetailtVehicleInspection(id);
                switch (type)
                {
                    case 1:
                        data = await _allCodeRepository.GetListSortByName(AllCodeType.VEHICLE_STATUS);
                        ViewBag.StatusCar = detail.VehicleStatus;
                        break;
                    case 2:
                        data = await _allCodeRepository.GetListSortByName(AllCodeType.LOAD_TYPE);
                        ViewBag.StatusCar = detail.LoadType;
                        break;
                    case 3:
                        data = await _allCodeRepository.GetListSortByName(AllCodeType.VEHICLEWEIGHING_TYPE);
                        ViewBag.StatusCar = detail.VehicleWeighingType;
                        break;
                    case 4:
                        data = await _allCodeRepository.GetListSortByName(AllCodeType.TROUGH_TYPE);
                        ViewBag.StatusCar = detail.TroughType;
                        break;
                    case 5:
                        data = await _allCodeRepository.GetListSortByName(AllCodeType.VEHICLETROUG_HWEIGHT);
                        ViewBag.StatusCar = detail.VehicleTroughWeight;
                        break;
                    case 6:
                        data = await _allCodeRepository.GetListSortByName(AllCodeType.VEHICLETROUGH_STATUS);
                        ViewBag.StatusCar = detail.VehicleTroughStatus;
                        break;
                    case 7:
                        data = await _allCodeRepository.GetListSortByName(AllCodeType.VEHICLEWEIGHINGSTATUS);
                        ViewBag.StatusCar = detail.VehicleWeighingStatus;
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
                LogHelper.InsertLogTelegram("ProcessingIsLoading - CarController: " + ex);
            }
            return View();
        }
        public async Task<IActionResult> ListProcessingIsLoading(CartoFactorySearchModel SearchModel)
        {
            try
            {
                var data = await _vehicleInspectionRepository.GetListCartoFactory(SearchModel);
                return PartialView(data);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("ListProcessingIsLoading - CarController: " + ex);
            }
            return PartialView();
        }
        public IActionResult CallTheScale()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("CallTheScale - CarController: " + ex);
            }
            return View();
        }
        public async Task<IActionResult> ListCallTheScale(CartoFactorySearchModel SearchModel)
        {
            try
            {
                ViewBag.type = SearchModel.type;
                var data = await _vehicleInspectionRepository.GetListCartoFactory(SearchModel);
                return PartialView(data);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("ListCallTheScale - CarController: " + ex);
            }
            return PartialView();
        }
        public IActionResult WeighedInput()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("WeighedInput - CarController: " + ex);
            }
            return View();
        }
        public async Task<IActionResult> ListWeighedInput(CartoFactorySearchModel SearchModel)
        {
            try
            {
                var data = await _vehicleInspectionRepository.GetListCartoFactory(SearchModel);
                if (data != null)
                {
                    data = data.OrderBy(s => s.LoadType).ToList();
                }
                return PartialView(data);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("ListWeighedInput - CarController: " + ex);
            }
            return PartialView();
        }
        public async Task<IActionResult> UpdateStatus(int id,int status, int type)
        {
            try
            {
                ViewBag.Id = id;
                ViewBag.StatusCar = 0;
                var data = new List<AllCode>();
                var detail = await _vehicleInspectionRepository.GetDetailtVehicleInspection(id);
                switch (type)
                {
                    case 1:
                        data = await _allCodeRepository.GetListSortByName(AllCodeType.VEHICLE_STATUS);
                        ViewBag.StatusCar = detail.VehicleStatus;
                        break;
                    case 2:
                        data = await _allCodeRepository.GetListSortByName(AllCodeType.LOAD_TYPE);
                        ViewBag.StatusCar = detail.LoadType;
                        break;
                    case 3:
                        data = await _allCodeRepository.GetListSortByName(AllCodeType.VEHICLEWEIGHING_TYPE);
                        ViewBag.StatusCar = detail.VehicleWeighingType;
                        break;
                    case 4:
                        data = await _allCodeRepository.GetListSortByName(AllCodeType.TROUGH_TYPE);
                        ViewBag.StatusCar = detail.TroughType;
                        break;
                    case 5:
                        data = await _allCodeRepository.GetListSortByName(AllCodeType.VEHICLETROUG_HWEIGHT);
                        ViewBag.StatusCar = detail.VehicleTroughWeight;
                        break;
                    case 6:
                        data = await _allCodeRepository.GetListSortByName(AllCodeType.VEHICLETROUGH_STATUS);
                        ViewBag.StatusCar = detail.VehicleTroughStatus;
                        break;
                    case 7:
                        data = await _allCodeRepository.GetListSortByName(AllCodeType.VEHICLEWEIGHINGSTATUS);
                        ViewBag.StatusCar = detail.VehicleWeighingStatus;
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
    }
}

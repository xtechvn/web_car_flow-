using Entities.Models;
using Entities.ViewModels.Car;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;
using Repositories.IRepositories;
using Repositories.Repositories;
using System.Security.Claims;
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
                var AllCode = await _allCodeRepository.GetListSortByName(AllCodeType.LOADINGSTATUS);
                ViewBag.AllCode = AllCode;
                var AllCode2 = await _allCodeRepository.GetListSortByName(AllCodeType.LOAD_TYPE);
                ViewBag.AllCode2 = AllCode2;
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
                var AllCode = await _allCodeRepository.GetListSortByName(AllCodeType.VEHICLEWEIGHING_TYPE);
                ViewBag.AllCode = AllCode;
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
                var AllCode = await _allCodeRepository.GetListSortByName(AllCodeType.TROUGH_TYPE );
                ViewBag.AllCode = AllCode;
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
        public async Task<IActionResult> UpdateStatus(int id, int status, int type)
        {
            try
            {
                var _UserId = 0;
                if (HttpContext.User.FindFirst(ClaimTypes.NameIdentifier) != null)
                {
                    _UserId = Convert.ToInt32(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
                }
                var UpdateCar = 0;
                ViewBag.Id = id;
                ViewBag.StatusCar = 0;
                var model = new VehicleInspectionUpdateModel();
                var detail = await _vehicleInspectionRepository.GetDetailtVehicleInspection(id);
                model.Id = detail.Id;
                model.RecordNumber = detail.RecordNumber;
                model.CustomerName = detail.CustomerName;
                model.VehicleNumber = detail.VehicleNumber;
                model.RegisterDateOnline = detail.RegisterDateOnline;
                model.DriverName = detail.DriverName;
                model.LicenseNumber = detail.LicenseNumber;
                model.PhoneNumber = detail.PhoneNumber;
                model.VehicleLoad = detail.VehicleLoad;
                model.VehicleStatus = detail.VehicleStatus;
                model.LoadType = detail.LoadType;
                model.IssueCreateDate = detail.IssueCreateDate;
                model.IssueUpdatedDate = detail.IssueUpdatedDate;
                model.VehicleWeighingType = detail.VehicleWeighingType;
                model.VehicleWeighingTimeComeIn = detail.VehicleWeighingTimeComeIn;
                model.VehicleWeighingTimeComeOut = detail.VehicleWeighingTimeComeOut;
                model.VehicleWeighingTimeComplete = detail.VehicleWeighingTimeComplete;
                model.TroughType = detail.TroughType;
                model.VehicleTroughTimeComeIn = detail.VehicleTroughTimeComeIn;
                model.VehicleTroughTimeComeOut = detail.VehicleTroughTimeComeOut;
                model.VehicleTroughWeight = detail.VehicleTroughWeight;
                model.VehicleTroughStatus = detail.VehicleTroughStatus;
                model.LoadingStatus = detail.LoadingStatus;
                model.UpdatedBy = _UserId;
                switch (type)
                {
                    case 1:
                        {
                            model.VehicleStatus = status;
                            UpdateCar = await _vehicleInspectionRepository.UpdateCar(model);
                        }
                        break;
                    case 2:
                        {
                            model.LoadType = status;
                            UpdateCar = await _vehicleInspectionRepository.UpdateCar(model);
                        }
                        break;
                    case 3:
                        {
                          
                            model.VehicleWeighingType = status;
                            UpdateCar = await _vehicleInspectionRepository.UpdateCar(model);
                        }
                        break;
                    case 4:
                        {
                            model.TroughType = status;
                            UpdateCar = await _vehicleInspectionRepository.UpdateCar(model);
                        }
                        break;
                    case 5:
                        {
                           
                            model.VehicleTroughWeight = status;
                            UpdateCar = await _vehicleInspectionRepository.UpdateCar(model);
                        }
                        break;
                    case 6:
                        {
                           
                            model.VehicleTroughStatus = status;
                            UpdateCar = await _vehicleInspectionRepository.UpdateCar(model);
                        }
                        break;
                    case 7:
                        {
                            
                            model.VehicleWeighingType = status;
                            UpdateCar = await _vehicleInspectionRepository.UpdateCar(model);
                        }
                        break; 
                    case 8:
                        {
                            model.LoadingStatus = status;
                            UpdateCar = await _vehicleInspectionRepository.UpdateCar(model);
                        }
                        break;
                }
                if (UpdateCar > 0)
                    return Ok(new
                    {
                        status = (int)ResponseType.SUCCESS,
                        msg = "cập nhật thành công"
                    });
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("OpenPopup - CarController: " + ex);
            }
            return Ok(new
            {
                status = (int)ResponseType.SUCCESS,
                msg = "cập nhật không thành công"
            });
        }
    }
}

using Entities.Models;
using Entities.ViewModels.Car;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Repositories.IRepositories;
using Repositories.Repositories;
using System.Threading.Tasks;
using Utilities.Contants;

namespace WEB.CMS.Controllers
{
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
            return View();
        } 
        public async Task<IActionResult> ListCartoFactory(CartoFactorySearchModel SearchModel)
        {
            var data =await _vehicleInspectionRepository.GetListCartoFactory(SearchModel);
            return PartialView(data);
        }
        public async Task<IActionResult> OpenPopup(int id, int type,int status)
        {
            ViewBag.Id = id;
            ViewBag.StatusCar = status;
            var data = new List<AllCode>();
            switch(type)
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
            return PartialView();
        }
    }
}

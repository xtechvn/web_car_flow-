using Entities.ViewModels.Car;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.IRepositories
{
    public interface IVehicleInspectionRepository
    {
        Task<List<CartoFactoryModel>> GetListCartoFactory(CartoFactorySearchModel searchModel);
        Task<CartoFactoryModel> GetDetailtVehicleInspection(int id);
    }
}

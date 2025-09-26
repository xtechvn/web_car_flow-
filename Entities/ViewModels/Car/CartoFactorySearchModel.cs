using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.ViewModels.Car
{
    public class CartoFactorySearchModel
    {
        public string? VehicleNumber { get; set; }
        public string? PhoneNumber { get; set; }
        public string? VehicleStatus { get; set; }
        public string? LoadType { get; set; }
        public string? VehicleWeighingType { get; set; }
        public string? VehicleTroughStatus { get; set; }
        public string? TroughType { get; set; }
        public string? VehicleWeighingStatus { get; set; }
        public int? type { get; set; }
        public int? LoadingStatus { get; set; }
        public int? VehicleWeighedstatus { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.ViewModels.Car
{
    public class CartoFactorySearchModel
    {
        public string? VehicleNumber { get; set; }        // Biển số xe
        public string? PhoneNumber { get; set; }          // Số điện thoại
        public string? VehicleStatus { get; set; }        // Trạng thái xe đến
        public string? LoadType { get; set; }             // Loại xanh/thuong
        public string? VehicleWeighingType { get; set; }  // Trạng thái gọi xe vào cân
        public string? VehicleTroughStatus { get; set; }  // Trạng thái  xe đã vào máng
        public string? TroughType { get; set; }           // Loại máng
        public string? VehicleWeighingStatus { get; set; } // Trạng thái  xe đã cân ra
        public int? type { get; set; }                    // Loại 1 đã sử lý,0 chưa sl
        public int? LoadingStatus { get; set; }           // Trạng sử lý đang tải
        public int? VehicleWeighedstatus { get; set; }    // Trạng thái xe đã được cân đầu vào
    }
}

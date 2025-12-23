using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Entities.ViewModels.Car
{
    public class CartoFactoryModel: VehicleInspection
    {
        public string LoadTypeName { get; set; }
        public string VehicleStatusName { get; set; }
        public string VehicleWeighingTypeName { get; set; }
        public string TroughTypeName { get; set; }
        public string VehicleTroughStatusName { get; set; }
        public string VehicleWeighingStatusName { get; set; }
        public string LoadingStatusName { get; set; }
        public string VehicleWeighedstatusName { get; set; }
        public string AudioPath { get; set; }
        public string Note { get; set; }
        public string LoadingTypeName { get; set; }
        public string FullName { get; set; }

    }
    public class CartoFactoryResponseDto
    {
        public int Id { get; set; }
        public int? RecordNumber { get; set; }
        public string CustomerName { get; set; }
        public string VehicleNumber { get; set; }
        public DateTime? RegisterDateOnline { get; set; }
        public string DriverName { get; set; }
        public string LicenseNumber { get; set; }
        public string PhoneNumber { get; set; }
        public string VehicleLoad { get; set; }
        public int? VehicleStatus { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string VehicleStatusName { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string AudioPath { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string LoadTypeName { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string VehicleWeighingStatusName { get; set; }
    }
}

﻿using System;
using System.Collections.Generic;

namespace Entities.Models
{
    public partial class VehicleInspection
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
        public int? LoadType { get; set; }
        public DateTime? IssueCreateDate { get; set; }
        public DateTime? IssueUpdatedDate { get; set; }
        public int? VehicleWeighingType { get; set; }
        public DateTime? VehicleWeighingTimeComeIn { get; set; }
        public DateTime? VehicleWeighingTimeComeOut { get; set; }
        public DateTime? VehicleWeighingTimeComplete { get; set; }
        public int? TroughType { get; set; }
        public DateTime? VehicleTroughTimeComeIn { get; set; }
        public DateTime? VehicleTroughTimeComeOut { get; set; }
        public decimal? VehicleTroughWeight { get; set; }
        public int? VehicleTroughStatus { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? VehicleWeighingStatus { get; set; }
        public int? LoadingStatus { get; set; }
        public int? VehicleWeighedstatus { get; set; }
        public DateTime? TimeCallVehicleTroughTimeComeIn { get; set; }
        public string AudioPath { get; set; }
    }
}

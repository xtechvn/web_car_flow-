﻿using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    }
}

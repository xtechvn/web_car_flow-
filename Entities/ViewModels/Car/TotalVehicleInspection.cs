using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.ViewModels.Car
{
    public class TotalVehicleInspection
    {
        public int TotalCar { get; set; }//Tổng số xe theo ngày
        public int TotalCarCompleted { get; set; }//Tổng số xe đã lấy hàng
        public int TotalCarUnfinished { get; set; }//Tổng số xe chưa lấy hàng
        public int TotalCarArriving16h { get; set; }//Tổng số xe đến lấy hàng sau 16h
        public int TotalTroughType3 { get; set; }//Tổng số bản ghi theo từng loại máng
        public int TotalTroughType4 { get; set; }
        public int TotalTroughType5 { get; set; }
        public int TotalTroughType6 { get; set; }
        public int TotalTroughType7 { get; set; }
        public int TotalTroughType8 { get; set; }
        public int TotalWeightTroughType { get; set; }//TÍNH TỔNG TRỌNG LƯỢNG
        public int TotalWeightTroughType3 { get; set; }
        public int TotalWeightTroughType4 { get; set; }
        public int TotalWeightTroughType5 { get; set; }
        public int TotalWeightTroughType6 { get; set; }
        public int TotalWeightTroughType7 { get; set; }
        public int TotalWeightTroughType8 { get; set; }

    }
}

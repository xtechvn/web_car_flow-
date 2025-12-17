namespace Web.Cargill.Api.Model
{
    public class DailyVehicleReportDto
    {
        public int TotalCar { get; set; }
        public int TotalCarCompleted { get; set; }
        public int TotalCarUnfinished { get; set; }
        public int TotalCarArriving16h { get; set; }

        public decimal TotalTimeInHour { get; set; }
        public decimal AvgTimePerCompletedCar_Hour { get; set; }
        public decimal TotalTimeWorkInHour { get; set; }
        public decimal AverageProductivity { get; set; }

        public decimal TotalWeightTroughType { get; set; }

        public int TotalTroughType3 { get; set; }
        public int TotalTroughType4 { get; set; }
        public int TotalTroughType5 { get; set; }
        public int TotalTroughType6 { get; set; }
        public int TotalTroughType7 { get; set; }
        public int TotalTroughType8 { get; set; }

        public decimal TotalWeightTroughType3 { get; set; }
        public decimal TotalWeightTroughType4 { get; set; }
        public decimal TotalWeightTroughType5 { get; set; }
        public decimal TotalWeightTroughType6 { get; set; }
        public decimal TotalWeightTroughType7 { get; set; }
        public decimal TotalWeightTroughType8 { get; set; }
    }
    public class DailyReportDto
    {
        public int TotalCar { get; set; }
        public int TotalCarCompleted { get; set; }
        public int TotalCarUnfinished { get; set; }
        public int TotalCarArriving16h { get; set; }

        public decimal TotalTimeInHour { get; set; }
        public decimal AvgTimePerCompletedCar_Hour { get; set; }

        public decimal TotalWeightTroughType { get; set; }

        public int TotalTroughType3 { get; set; }
        public int TotalTroughType4 { get; set; }
        public int TotalTroughType5 { get; set; }
        public int TotalTroughType6 { get; set; }
        public int TotalTroughType7 { get; set; }
        public int TotalTroughType8 { get; set; }

        public decimal AverageProductivity { get; set; }
    }


}

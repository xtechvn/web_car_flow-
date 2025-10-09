namespace Web.Cargill.Api.Model
{
    public class VehicleInspection
    {
        public int Id { get; set; }
        public string RecordNumber { get; set; }
        public string CustomerName { get; set; }
        public string VehicleNumber { get; set; }
        public DateTime? RegisterDateOnline { get; set; }
        public string DriverName { get; set; }
        public string LicenseNumber { get; set; }
        public string PhoneNumber { get; set; }
        public decimal? VehicleLoad { get; set; }
        public string VehicleStatus { get; set; }
        public string LoadType { get; set; }
        public DateTime? IssueCreateDate { get; set; }
        public DateTime? IssueUpdatedDate { get; set; }
        public string VehicleWeighingType { get; set; }
        public DateTime? VehicleWeighingTimeComeIn { get; set; }
        public DateTime? VehicleWeighingTimeComeOut { get; set; }
        public DateTime? VehicleWeighingTimeComplete { get; set; }
        public string TroughType { get; set; }
        public DateTime? VehicleTroughTimeComeIn { get; set; }
        public DateTime? VehicleTroughTimeComeOut { get; set; }
        public decimal? VehicleTroughWeight { get; set; }
        public string VehicleTroughStatus { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string VehicleWeighingStatus { get; set; }
        public string LoadingStatus { get; set; }
        public string VehicleWeighedstatus { get; set; }
        public DateTime? TimeCallVehicleTroughTimeComeIn { get; set; }

        // Thêm field AudioUrl để lưu link audio
        public string AudioUrl { get; set; }
    }
}

using Aspose.Cells;
using DAL;
using Entities.ConfigModels;
using Entities.Models;
using Entities.ViewModels.Car;
using Microsoft.Extensions.Options;
using Nest;
using Repositories.IRepositories;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;
using Utilities.Contants;

namespace Repositories.Repositories
{
    public class VehicleInspectionRepository: IVehicleInspectionRepository
    {
        private readonly VehicleInspectionDAL _VehicleInspectionDAL;
        private readonly IOptions<DataBaseConfig> dataBaseConfig;
        public VehicleInspectionRepository(IOptions<DataBaseConfig> _dataBaseConfig) {
            dataBaseConfig = _dataBaseConfig;
            _VehicleInspectionDAL = new VehicleInspectionDAL(dataBaseConfig.Value.SqlServer.ConnectionString);
        }
        public async Task<List<CartoFactoryModel>> GetListCartoFactory(CartoFactorySearchModel searchModel)
        {
            try
            {
                var now = DateTime.Now;
                var expireAt = new DateTime(now.Year, now.Month, now.Day, 17, 55, 0);
                if (now >= expireAt)
                {
                    searchModel.RegistrationTime = expireAt;
                }
                else
                {
                    searchModel.RegistrationTime = expireAt.AddDays(-1);
                }
                return await _VehicleInspectionDAL.GetListCartoFactory(searchModel);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetListCartoFactory - VehicleInspectionRepository: " + ex);
            }
            return null;
        }
        public async Task<int> UpdateCar(VehicleInspectionUpdateModel model)
        {
            try
            {
                return await _VehicleInspectionDAL.UpdateVehicleInspection(model);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("UpdateVehicleInspection - VehicleInspectionRepository: " + ex);
                return -1;
            }
        }

        public async Task<CartoFactoryModel> GetDetailtVehicleInspection(int id)
        {
            try
            {
                return await _VehicleInspectionDAL.GetDetailtVehicleInspection(id);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetListCartoFactory - VehicleInspectionRepository: " + ex);
            }
            return null;
        }
        public int SaveVehicleInspection(RegistrationRecord model)
        {
            try
            {
                return  _VehicleInspectionDAL.SaveVehicleInspection(model);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("SaveVehicleInspection - VehicleInspectionRepository: " + ex);
            }
            return 0;
        }  
        public Task<string> GetAudioPathByVehicleNumber(string VehicleNumber)
        {
            try
            {
                return  _VehicleInspectionDAL.GetAudioPathByVehicleNumber(VehicleNumber);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("SaveVehicleInspection - VehicleInspectionRepository: " + ex);
            }
            return null;
        }
        public async Task<List<CartoFactoryModel>> GetListVehicleInspectionSynthetic(DateTime? FromDate, DateTime? ToDate, int LoadType)
        {
            try
            {
  
                return await _VehicleInspectionDAL.GetListVehicleInspectionSynthetic(FromDate,ToDate, LoadType);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetListVehicleInspectionSynthetic - VehicleInspectionRepository: " + ex);
            }
            return null;
        }    
        public async Task<TotalVehicleInspection> CountTotalVehicleInspectionSynthetic(DateTime? FromDate, DateTime? ToDate)
        {
            try
            {
             
                return await _VehicleInspectionDAL.CountTotalVehicleInspectionSynthetic(FromDate,ToDate);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetListVehicleInspectionSynthetic - VehicleInspectionRepository: " + ex);
            }
            return null;
        }
        public async Task<List<TotalWeightByHourModel>> GetTotalWeightByHour(DateTime? RegistrationTime)
        {
            try
            {

                return await _VehicleInspectionDAL.GetTotalWeightByHour(RegistrationTime);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetTotalWeightByHour - VehicleInspectionRepository: " + ex);
            }
            return null;
        }    
        public async Task<List<TotalWeightByWeightGroupModel>> GetTotalWeightByWeightGroup(DateTime? RegistrationTime)
        {
            try
            {

                return await _VehicleInspectionDAL.GetTotalWeightByWeightGroup(RegistrationTime);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetTotalWeightByHour - VehicleInspectionRepository: " + ex);
            }
            return null;
        }    
        public async Task<List<TotalWeightByTroughTypeModel>> GetTotalWeightByTroughType(DateTime? RegistrationTime)
        {
            try
            {

                return await _VehicleInspectionDAL.GetTotalWeightByTroughType(RegistrationTime);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetTotalWeightByTroughType - VehicleInspectionRepository: " + ex);
            }
            return null;
        }
        public async Task<List<CartoFactoryModel>> SearchVehicleInspection(CartoFactorySearchModel searchModel)
        {
            try
            {
                return await _VehicleInspectionDAL.GetListCartoFactory(searchModel);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("SearchVehicleInspection - VehicleInspectionRepository: " + ex);
                return null;
            }
        }
        public Task<string> GetAudioPathByVehicleNumberAPI(string VehicleNumber, int LocationType=0)
        {
            try
            {
                switch(LocationType)
                {
                    case 0:
                    case 1:
                        {
                            VehicleInspectionDAL _VehicleInspectionDAL = new VehicleInspectionDAL(dataBaseConfig.Value.SqlServer.ConnectionString);
                            return _VehicleInspectionDAL.GetAudioPathByVehicleNumber(VehicleNumber);
                        }
                    case 2:
                        {
                            VehicleInspectionDAL _VehicleInspectionDAL_LongAn = new VehicleInspectionDAL(dataBaseConfig.Value.SqlServer.ConnectionString_LongAn);
                            return _VehicleInspectionDAL_LongAn.GetAudioPathByVehicleNumber(VehicleNumber);
                        }                   
                    case 3:
                        {
                            VehicleInspectionDAL _VehicleInspectionDAL_BinhDinh = new VehicleInspectionDAL(dataBaseConfig.Value.SqlServer.ConnectionString_BinhDinh);
                            return _VehicleInspectionDAL_BinhDinh.GetAudioPathByVehicleNumber(VehicleNumber);
                        }
                    case 4:
                        {
                            VehicleInspectionDAL _VehicleInspectionDAL_BinhDuong = new VehicleInspectionDAL(dataBaseConfig.Value.SqlServer.ConnectionString_BinhDuong);
                            return _VehicleInspectionDAL_BinhDuong.GetAudioPathByVehicleNumber(VehicleNumber);
                        }
                    default:
                        break;
                }
                
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("SaveVehicleInspection - VehicleInspectionRepository: " + ex);
            }
            return null;
        }
        public int SaveVehicleInspectionAPI(RegistrationRecord model)
        {
            try
            {
                switch(model.LocationType)
                {
                    case 0:
                    case 1:
                        {
                            VehicleInspectionDAL _VehicleInspectionDAL = new VehicleInspectionDAL(dataBaseConfig.Value.SqlServer.ConnectionString);
                            return _VehicleInspectionDAL.SaveVehicleInspection(model);

                        }
                    case 2:
                        {
                            VehicleInspectionDAL _VehicleInspectionDAL_LongAn = new VehicleInspectionDAL(dataBaseConfig.Value.SqlServer.ConnectionString_LongAn);
                            return _VehicleInspectionDAL_LongAn.SaveVehicleInspection(model);
                        }
                    case 3:
                        {
                            VehicleInspectionDAL _VehicleInspectionDAL_BinhDinh = new VehicleInspectionDAL(dataBaseConfig.Value.SqlServer.ConnectionString_BinhDinh);
                            return _VehicleInspectionDAL_BinhDinh.SaveVehicleInspection(model);
                        }
                    case 4:
                        {
                            VehicleInspectionDAL _VehicleInspectionDAL_BinhDuong = new VehicleInspectionDAL(dataBaseConfig.Value.SqlServer.ConnectionString_BinhDuong);
                            return _VehicleInspectionDAL_BinhDuong.SaveVehicleInspection(model);
                        }
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("SaveVehicleInspection - VehicleInspectionRepository: " + ex);
            }
            return 0;
        }
        public async Task<int> UpdateVehicleInspectionByVehicleNumber(string VehicleNumber)
        {
            try
            {
                VehicleInspectionDAL _VehicleInspectionDAL_LongAn = new VehicleInspectionDAL(dataBaseConfig.Value.SqlServer.ConnectionString_LongAn);
                return await _VehicleInspectionDAL_LongAn.UpdateVehicleInspectionByVehicleNumber(VehicleNumber);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("SaveVehicleInspection - VehicleInspectionRepository: " + ex);
            }
            return 0;
        }
        public List<VehicleInspection> checkVehicleInspectionbyRegisterDateOnline(DateTime? RegisterDateOnline, string VehicleNumber)
        {
            try
            {
                return _VehicleInspectionDAL.checkVehicleInspectionbyRegisterDateOnline(RegisterDateOnline, VehicleNumber);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("checkVehicleInspectionbyRegisterDateOnline - VehicleInspectionRepository: " + ex);
                return null;

            }
        }
        public async Task<string> ExportSummaryReport(List<CartoFactoryModel> data, string FilePath)
        {
            var pathResult = string.Empty;
            try
            {
                if (data != null && data.Count > 0)
                {
                    Workbook wb = new Workbook();
                    Worksheet ws = wb.Worksheets[0];
                    ws.Name = "Danh sách xe";
                    Cells cell = ws.Cells;

                    var range = ws.Cells.CreateRange(0, 0, 1, 1);
                    StyleFlag st = new StyleFlag();
                    st.All = true;
                    Style style = ws.Cells["A1"].GetStyle();

                    #region Header
                    range = cell.CreateRange(0, 0, 1, 19);
                    style = ws.Cells["A1"].GetStyle();
                    style.Font.IsBold = true;
                    style.IsTextWrapped = true;
                    style.ForegroundColor = Color.FromArgb(33, 88, 103);
                    style.BackgroundColor = Color.FromArgb(33, 88, 103);
                    style.Pattern = BackgroundType.Solid;
                    style.Font.Color = Color.White;
                    style.VerticalAlignment = TextAlignmentType.Center;
                    style.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
                    style.Borders[BorderType.TopBorder].Color = Color.Black;
                    style.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;
                    style.Borders[BorderType.BottomBorder].Color = Color.Black;
                    style.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin;
                    style.Borders[BorderType.LeftBorder].Color = Color.Black;
                    style.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
                    style.Borders[BorderType.RightBorder].Color = Color.Black;
                    range.ApplyStyle(style, st);

                    // Set column width
                    cell.SetColumnWidth(0, 8);
                    cell.SetColumnWidth(1, 20);
                    cell.SetColumnWidth(2, 40);
                    cell.SetColumnWidth(3, 20);
                    cell.SetColumnWidth(4, 20);
                    cell.SetColumnWidth(5, 30);
                    cell.SetColumnWidth(6, 30);
                    cell.SetColumnWidth(7, 25);
                    cell.SetColumnWidth(8, 25);
                    cell.SetColumnWidth(9, 25);
                    cell.SetColumnWidth(10, 25);
                    cell.SetColumnWidth(11, 25);
                    cell.SetColumnWidth(12, 25);
                    cell.SetColumnWidth(13, 25);
                    cell.SetColumnWidth(14, 25);
                    cell.SetColumnWidth(15, 25);
                    cell.SetColumnWidth(16, 25);
                    cell.SetColumnWidth(17, 25);
                    cell.SetColumnWidth(18, 25);
                    cell.SetColumnWidth(19, 25);




                    // Set header value
                    ws.Cells["A1"].PutValue("STT");
                    ws.Cells["B1"].PutValue("Giờ đăng ký online");
                    ws.Cells["C1"].PutValue("Tên KH (Trại, đại lý)");
                    ws.Cells["D1"].PutValue("Tên lái xe");
                    ws.Cells["E1"].PutValue("Số điện thoại");
                    ws.Cells["F1"].PutValue("Biển số xe");
                    ws.Cells["G1"].PutValue("Nhóm tài");
                    ws.Cells["H1"].PutValue("Giờ xe có mặt");
                    ws.Cells["I1"].PutValue("Giờ vào cân");
                    ws.Cells["J1"].PutValue("Giờ cân xong đầu vào");
                    ws.Cells["K1"].PutValue("Giờ vào máng");
                    ws.Cells["L1"].PutValue("Giờ ra máng");
                    ws.Cells["M1"].PutValue("Giờ cân xong đầu ra");
                    ws.Cells["N1"].PutValue("Thời gian xuất hàng(Phút)");
                    ws.Cells["O1"].PutValue("Tình trạng");
                    ws.Cells["P1"].PutValue("Tổng trọng lượng đã lấy(KG)");
                    ws.Cells["Q1"].PutValue("CSOS");

                    #endregion

                    #region Body

                    range = cell.CreateRange(1, 0, data.Count * 2, 19);
                    style = ws.Cells["A2"].GetStyle();
                    style.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
                    style.Borders[BorderType.TopBorder].Color = Color.Black;
                    style.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;
                    style.Borders[BorderType.BottomBorder].Color = Color.Black;
                    style.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin;
                    style.Borders[BorderType.LeftBorder].Color = Color.Black;
                    style.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
                    style.Borders[BorderType.RightBorder].Color = Color.Black;
                    style.VerticalAlignment = TextAlignmentType.Center;
                    range.ApplyStyle(style, st);

                    Style alignCenterStyle = ws.Cells["A2"].GetStyle();
                    alignCenterStyle.HorizontalAlignment = TextAlignmentType.Center;

                    Style numberStyle = ws.Cells["A2"].GetStyle();
                    numberStyle.Number = 3;
                    numberStyle.HorizontalAlignment = TextAlignmentType.Right;
                    numberStyle.VerticalAlignment = TextAlignmentType.Center;

                    int RowIndex = 2;

                    foreach (var item in data)
                    {
                        
                        var TrangThai_name = "";
                        // Ưu tiên kiểm tra từ Bước 6 xuống Bước 1 
                        if ((item.VehicleTroughStatus == (int)VehicleTroughStatus.Hoan_thanh) && (item.VehicleWeighingStatus != null))
                        {
                            TrangThai_name = "Bước 6";
                        }
                        else if ((item.VehicleWeighedstatus == (int)VehicleWeighedstatus.Da_Can_Xong_Dau_Cao) && item.TroughType != null &&
                        (item.VehicleTroughStatus != null || item.VehicleTroughStatus == (int)VehicleTroughStatus.Blank))
                        {
                            TrangThai_name = "Bước 5";
                        }
                        else if ((item.VehicleWeighingType == (int)VehicleWeighingType.DA_Vao_Can) &&
                        (item.VehicleWeighedstatus != null || item.VehicleWeighedstatus == (int)VehicleWeighedstatus.Blank))
                        {
                            TrangThai_name = "Bước 4";
                        }
                        else if ((item.LoadingStatus == (int)LoadingStatus.Da_HTTC) &&
                        (item.VehicleWeighingType != null || item.VehicleWeighingType == (int)VehicleWeighingType.Blank))
                        {
                            TrangThai_name = "Bước 3";
                        }
                        else if ((item.VehicleStatus == (int)VehicleStatus.Da_Den_NM) &&
                        (item.LoadingStatus != null || item.LoadingStatus == (int)LoadingStatus.Blank))
                        {
                            TrangThai_name = "Bước 2";
                        }
                        else if (item.VehicleStatus != null && (item.LoadingStatus == null || item.LoadingStatus == (int)VehicleWeighingType.Blank))
                        {
                            TrangThai_name = "Bước 1";
                        }
                        var totalMinutes = item.VehicleWeighingTimeComplete.HasValue && item.VehicleWeighingTimeComeIn.HasValue ? (item.VehicleWeighingTimeComplete.Value - item.VehicleWeighingTimeComeIn.Value).TotalMinutes : 0;

                        string ttchitiet = string.Empty;

                        ws.Cells["A" + RowIndex].PutValue(item.RecordNumber);
                        ws.Cells["B" + RowIndex].PutValue(((DateTime)item.RegisterDateOnline).ToString("dd/MM/yyyy HH:mm"));
                        ws.Cells["C" + RowIndex].PutValue(item.CustomerName);
                        ws.Cells["D" + RowIndex].PutValue(item.DriverName);
                        ws.Cells["E" + RowIndex].PutValue(item.PhoneNumber);
                        ws.Cells["F" + RowIndex].PutValue(item.VehicleNumber);
                        ws.Cells["G" + RowIndex].PutValue(item.LoadTypeName);
                        ws.Cells["H" + RowIndex].PutValue(item.VehicleArrivalDate != null ? item.VehicleArrivalDate.Value.ToString("HH:mm dd/MM/yyyy") : "");
                        ws.Cells["I" + RowIndex].PutValue(item.VehicleWeighingTimeComeIn != null ? item.VehicleWeighingTimeComeIn.Value.ToString("HH:mm dd/MM/yyyy") : "");
                        ws.Cells["J" + RowIndex].PutValue(item.VehicleWeighingTimeComeOut != null ? item.VehicleWeighingTimeComeOut.Value.ToString("HH:mm dd/MM/yyyy") : "");
                        ws.Cells["K" + RowIndex].PutValue(item.VehicleTroughTimeComeIn != null ? item.VehicleTroughTimeComeIn.Value.ToString("HH:mm dd/MM/yyyy") : "");
                        ws.Cells["L" + RowIndex].PutValue(item.VehicleTroughTimeComeOut != null ? item.VehicleTroughTimeComeOut.Value.ToString("HH:mm dd/MM/yyyy") : "");
                        ws.Cells["M" + RowIndex].PutValue(item.VehicleWeighingTimeComplete != null ? item.VehicleWeighingTimeComplete.Value.ToString("HH:mm dd/MM/yyyy") : "");
                        ws.Cells["N" + RowIndex].PutValue((int)Math.Round(totalMinutes));
                        ws.Cells["O" + RowIndex].PutValue(TrangThai_name);
                        ws.Cells["P" + RowIndex].PutValue(item.VehicleTroughWeight);
                        ws.Cells["Q" + RowIndex].PutValue(item.FullName);


                        RowIndex++;

                    }

                    #endregion
                    wb.Save(FilePath);
                    pathResult = FilePath;
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("ExportDeposit - VehicleInspectionRepository: " + ex);
            }
            return pathResult;
        }
    }

}

using DAL.Generic;
using DAL.StoreProcedure;
using Entities.Models;
using Entities.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities.Contants;
using Utilities;
using Entities.ViewModels.Car;
using Microsoft.Data.SqlClient;
using System.Data;


namespace DAL
{
    public class VehicleInspectionDAL : GenericService<VehicleInspection>
    {
        private static DbWorker _DbWorker;
        public VehicleInspectionDAL(string connection) : base(connection)
        {
            _DbWorker = new DbWorker(connection);
        }
        public async Task<List<CartoFactoryModel>> GetListCartoFactory(CartoFactorySearchModel searchModel)
        {
            try
            {
                SqlParameter[] objParam = new SqlParameter[]
                {
                    new SqlParameter("@VehicleNumber", searchModel.VehicleNumber==null? DBNull.Value :searchModel.VehicleNumber),
                    new SqlParameter("@PhoneNumber", searchModel.PhoneNumber==null? DBNull.Value :searchModel.PhoneNumber),
                    new SqlParameter("@VehicleStatus", searchModel.VehicleStatus==null? DBNull.Value :searchModel.VehicleStatus),
                    new SqlParameter("@LoadType", searchModel.LoadType==null? DBNull.Value :searchModel.LoadType),
                    new SqlParameter("@VehicleWeighingType", searchModel.VehicleWeighingType==null? DBNull.Value :searchModel.VehicleWeighingType),
                    new SqlParameter("@VehicleTroughStatus", searchModel.VehicleTroughStatus==null? DBNull.Value :searchModel.VehicleTroughStatus),
                    new SqlParameter("@TroughType", searchModel.TroughType==null? DBNull.Value :searchModel.TroughType),
                    new SqlParameter("@VehicleWeighingStatus", searchModel.VehicleWeighingStatus==null? DBNull.Value :searchModel.VehicleWeighingStatus),
                };
                var dt = _DbWorker.GetDataTable(StoreProcedureConstant.SP_GetListVehicleInspection, objParam);
                if (dt != null && dt.Rows.Count > 0)
                {
                    return dt.ToList<CartoFactoryModel>();
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetListCartoFactory - VehicleInspectionDAL: " + ex);
            }
            return null;
        }
        public async Task<int> UpdateVehicleInspection(VehicleInspectionUpdateModel model)
        {
            try
            {
                SqlParameter[] objParam = new SqlParameter[]
                {
            new SqlParameter("@Id", model.Id),
            new SqlParameter("@RecordNumber", (object?)model.RecordNumber ?? DBNull.Value),
            new SqlParameter("@CustomerName", (object?)model.CustomerName ?? DBNull.Value),
            new SqlParameter("@VehicleNumber", (object?)model.VehicleNumber ?? DBNull.Value),
            new SqlParameter("@RegisterDateOnline", (object?)model.RegisterDateOnline ?? DBNull.Value),
            new SqlParameter("@DriverName", (object?)model.DriverName ?? DBNull.Value),
            new SqlParameter("@LicenseNumber", (object?)model.LicenseNumber ?? DBNull.Value),
            new SqlParameter("@PhoneNumber", (object?)model.PhoneNumber ?? DBNull.Value),
            new SqlParameter("@VehicleLoad", (object?)model.VehicleLoad ?? DBNull.Value),
            new SqlParameter("@VehicleStatus", (object?)model.VehicleStatus ?? DBNull.Value),
            new SqlParameter("@LoadType", (object?)model.LoadType ?? DBNull.Value),
            new SqlParameter("@IssueCreateDate", (object?)model.IssueCreateDate ?? DBNull.Value),
            new SqlParameter("@IssueUpdatedDate", (object?)model.IssueUpdatedDate ?? DBNull.Value),
            new SqlParameter("@VehicleWeighingType", (object?)model.VehicleWeighingType ?? DBNull.Value),
            new SqlParameter("@VehicleWeighingTimeComeIn", (object?)model.VehicleWeighingTimeComeIn ?? DBNull.Value),
            new SqlParameter("@VehicleWeighingTimeComeOut", (object?)model.VehicleWeighingTimeComeOut ?? DBNull.Value),
            new SqlParameter("@VehicleWeighingTimeComplete", (object?)model.VehicleWeighingTimeComplete ?? DBNull.Value),
            new SqlParameter("@TroughType", (object?)model.TroughType ?? DBNull.Value),
            new SqlParameter("@VehicleTroughTimeComeIn", (object?)model.VehicleTroughTimeComeIn ?? DBNull.Value),
            new SqlParameter("@VehicleTroughTimeComeOut", (object?)model.VehicleTroughTimeComeOut ?? DBNull.Value),
            new SqlParameter("@VehicleTroughWeight", (object?)model.VehicleTroughWeight ?? DBNull.Value),
            new SqlParameter("@VehicleTroughStatus", (object?)model.VehicleTroughStatus ?? DBNull.Value),
            new SqlParameter("@UpdatedBy", (object?)model.UpdatedBy ?? DBNull.Value),
           
                };

                // Gọi SP
                var dt = _DbWorker.GetDataTable(StoreProcedureConstant.sp_UpdateVehicleInspection, objParam);

                // Lấy giá trị Identity OUT
                int identity = (int)objParam.Last().Value;
                return identity;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("UpdateVehicleInspection - VehicleInspectionDAL: " + ex);
                return -1;
            }
        }

        public async Task<CartoFactoryModel> GetDetailtVehicleInspection(int id)
        {
            try
            {
                SqlParameter[] objParam = new SqlParameter[]
                {
                    new SqlParameter("@Id", id),
                   
                };
                var dt = _DbWorker.GetDataTable(StoreProcedureConstant.SP_GetDetailtVehicleInspection, objParam);
                if (dt != null && dt.Rows.Count > 0)
                {
                    var data= dt.ToList<CartoFactoryModel>();
                    return data.FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetDetailtVehicleInspection - VehicleInspectionDAL: " + ex);
            }
            return null;
        }

    }
}

﻿using DAL.Generic;
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
                    new SqlParameter("@LoadingStatus", searchModel.LoadingStatus==null? DBNull.Value :searchModel.LoadingStatus),
                    new SqlParameter("@VehicleWeighedstatus", searchModel.VehicleWeighedstatus==null? DBNull.Value :searchModel.VehicleWeighedstatus),
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
            new SqlParameter("@LoadingStatus", (object?)model.LoadingStatus ?? DBNull.Value),
            new SqlParameter("@VehicleWeighedstatus", (object?)model.VehicleWeighedstatus ?? DBNull.Value),
            new SqlParameter("@VehicleWeighingStatus", (object?)model.VehicleWeighingStatus ?? DBNull.Value),
            new SqlParameter("@TimeCallVehicleTroughTimeComeIn", (object?)model.TimeCallVehicleTroughTimeComeIn ?? DBNull.Value),
           
                };

                // Gọi SP
                var dt = _DbWorker.ExecuteNonQuery(StoreProcedureConstant.sp_UpdateVehicleInspection, objParam);

                return dt;
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
        public  int SaveVehicleInspection(RegistrationRecord model)
        {
            try
            {
                SqlParameter[] objParam_order = new SqlParameter[23];
                objParam_order[0] = new SqlParameter("@RecordNumber", model.QueueNumber);
                objParam_order[1] = new SqlParameter("@CustomerName", model.Name);
                objParam_order[2] = new SqlParameter("@VehicleNumber", model.PlateNumber);
                objParam_order[3] = new SqlParameter("@RegisterDateOnline", model.RegistrationTime);
                objParam_order[4] = new SqlParameter("@DriverName", model.GPLX);
                objParam_order[5] = new SqlParameter("@LicenseNumber", model.Camp);
                objParam_order[6] = new SqlParameter("@PhoneNumber", model.PhoneNumber);
                objParam_order[7] = new SqlParameter("@VehicleLoad", model.Referee);
                objParam_order[8] = new SqlParameter("@VehicleStatus", (int)VehicleStatus.Blank);
                objParam_order[9] = new SqlParameter("@LoadType", DBNull.Value);
                objParam_order[10] = new SqlParameter("@IssueCreateDate", DateTime.Now);
                objParam_order[11] = new SqlParameter("@IssueUpdatedDate", DBNull.Value);
                objParam_order[12] = new SqlParameter("@VehicleWeighingType", DBNull.Value);
                objParam_order[13] = new SqlParameter("@VehicleWeighingTimeComeIn", DBNull.Value);
                objParam_order[14] = new SqlParameter("@VehicleWeighingTimeComeOut", DBNull.Value);
                objParam_order[15] = new SqlParameter("@VehicleWeighingTimeComplete", DBNull.Value);
                objParam_order[16] = new SqlParameter("@TroughType", DBNull.Value);
                objParam_order[17] = new SqlParameter("@VehicleTroughTimeComeIn", DBNull.Value);
                objParam_order[18] = new SqlParameter("@VehicleTroughTimeComeOut", DBNull.Value);
                objParam_order[19] = new SqlParameter("@VehicleTroughWeight", DBNull.Value);
                objParam_order[20] = new SqlParameter("@VehicleTroughStatus", DBNull.Value);
                objParam_order[21] = new SqlParameter("@CreatedBy", 0);
                objParam_order[22] = new SqlParameter("@CreatedDate", DBNull.Value);



                var id = _DbWorker.ExecuteNonQuery("sp_InsertVehicleInspection", objParam_order);
                return id;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("SaveVehicleInspection - VehicleInspectionDAL: " + ex);
                return -1;
            }
        }
    }
}

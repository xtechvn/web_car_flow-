using App_Push_Consummer.Common;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App_Push_Consummer.Model.DB_Core
{
    public static class Repository
    {
        private static string tele_group_id = ConfigurationManager.AppSettings["tele_group_id"];
        private static string tele_token = ConfigurationManager.AppSettings["tele_token"];
        public static int SaveVehicleInspection(RegistrationRecord model)
        {
            try
            {
                SqlParameter[] objParam_order = new SqlParameter[24];
                objParam_order[0] = new SqlParameter("@RecordNumber", model.QueueNumber);
                objParam_order[1] = new SqlParameter("@CustomerName", model.Name);
                objParam_order[2] = new SqlParameter("@VehicleNumber", model.PlateNumber);
                objParam_order[3] = new SqlParameter("@RegisterDateOnline", model.RegistrationTime);
                objParam_order[4] = new SqlParameter("@DriverName", model.GPLX);
                objParam_order[5] = new SqlParameter("@LicenseNumber", model.Camp);
                objParam_order[6] = new SqlParameter("@PhoneNumber", model.PhoneNumber);
                objParam_order[7] = new SqlParameter("@VehicleLoad", model.Referee);
                objParam_order[8] = new SqlParameter("@VehicleStatus", DBNull.Value);
                objParam_order[9] = new SqlParameter("@LoadType", DBNull.Value);
                objParam_order[10] = new SqlParameter("@IssueCreateDate", DBNull.Value);
                objParam_order[12] = new SqlParameter("@IssueUpdatedDate", DBNull.Value);
                objParam_order[13] = new SqlParameter("@VehicleWeighingType", DBNull.Value);
                objParam_order[14] = new SqlParameter("@VehicleWeighingTimeComeIn", DBNull.Value);
                objParam_order[15] = new SqlParameter("@VehicleWeighingTimeComeOut", DBNull.Value);
                objParam_order[16] = new SqlParameter("@VehicleWeighingTimeComplete", DBNull.Value);
                objParam_order[17] = new SqlParameter("@TroughType", DBNull.Value);
                objParam_order[18] = new SqlParameter("@VehicleTroughTimeComeIn", DBNull.Value);
                objParam_order[19] = new SqlParameter("@VehicleTroughTimeComeOut", DBNull.Value);
                objParam_order[20] = new SqlParameter("@VehicleTroughWeight", DBNull.Value);
                objParam_order[21] = new SqlParameter("@VehicleTroughStatus", DBNull.Value);
                objParam_order[22] = new SqlParameter("@CreatedBy", DBNull.Value);
                objParam_order[23] = new SqlParameter("@CreatedDate", DBNull.Value);
              
          

                var id = DBWorker.ExecuteNonQuery("sp_InsertVehicleInspection", objParam_order);
                return id;
            }
            catch (Exception ex)
            {
                ErrorWriter.InsertLogTelegramByUrl(tele_token, tele_group_id, "Repository =>SaveVehicleInspection error queue = " + ex.ToString());
                return -1;
            }
        }
    }
}

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

    }
}

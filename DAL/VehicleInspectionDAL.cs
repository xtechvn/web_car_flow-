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
                    new SqlParameter("@VehicleNumber", searchModel.VehicleNumber),
                    new SqlParameter("@PhoneNumber", searchModel.PhoneNumber),
                    new SqlParameter("@VehicleStatus", searchModel.VehicleStatus),
                    new SqlParameter("@LoadType", searchModel.LoadType),
                    new SqlParameter("@VehicleWeighingType", searchModel.VehicleWeighingType),
                    new SqlParameter("@VehicleTroughStatus", searchModel.VehicleTroughStatus),
                    new SqlParameter("@TroughType", searchModel.TroughType),
                    new SqlParameter("@VehicleWeighingStatus", searchModel.VehicleWeighingStatus),
                };
                var dt = _DbWorker.GetDataTable(StoreProcedureConstant.SP_GetAllUser_search, objParam);
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

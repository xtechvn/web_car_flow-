using DAL;
using Entities.ConfigModels;
using Entities.Models;
using Entities.ViewModels.Car;
using Microsoft.Extensions.Options;
using Nest;
using Repositories.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;

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
        public Task<string> GetAudioPathByVehicleNumberAPI(string VehicleNumber, int LocationType)
        {
            try
            {
                switch(LocationType)
                {
                    case 1:
                        return _VehicleInspectionDAL.GetAudioPathByVehicleNumber(VehicleNumber);
                       
                    case 2:
                        {
                            VehicleInspectionDAL _VehicleInspectionDAL_LongAn = new VehicleInspectionDAL(dataBaseConfig.Value.SqlServer.ConnectionString_LongAn);
                            return _VehicleInspectionDAL_LongAn.GetAudioPathByVehicleNumber(VehicleNumber);
                        }
                    case 3:
                        VehicleNumber = VehicleNumber + "_3";
                        break;
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
                    case 1:
                        return _VehicleInspectionDAL.SaveVehicleInspection(model);
                    case 2:
                        {
                            VehicleInspectionDAL _VehicleInspectionDAL_LongAn = new VehicleInspectionDAL(dataBaseConfig.Value.SqlServer.ConnectionString_LongAn);
                            return _VehicleInspectionDAL_LongAn.SaveVehicleInspection(model);
                        }
                    default:
                        break;
                }
                return _VehicleInspectionDAL.SaveVehicleInspection(model);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("SaveVehicleInspection - VehicleInspectionRepository: " + ex);
            }
            return 0;
        }
    }
}

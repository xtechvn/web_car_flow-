﻿using DAL;
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
        public VehicleInspectionRepository(IOptions<DataBaseConfig> dataBaseConfig) {
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
    }
}

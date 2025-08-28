using App_Push_Consummer.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App_Push_Consummer.Interfaces
{
    public interface IGoogleSheetsService
    {
        Task<bool> SaveRegistrationAsync(RegistrationRecord record);
        Task<int> GetDailyQueueCountRedis();
    }
}

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
        Task<int> GetDailyQueueCountAsync();
        Task<bool> SaveRegistrationAsync(RegistrationRecord record);
        Task<DateTime?> GetLastSubmissionTimeAsync(string phoneNumber);
        Task UpdateLastSubmissionTimeAsync(string phoneNumber, DateTime submissionTime);
        Task<int> GetDailyQueueCountRedis();
    }
}

using App_Push_Consummer.Interfaces;
using App_Push_Consummer.Model;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace App_Push_Consummer.Google
{
    public class GoogleFormsService : IGoogleFormsService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<GoogleFormsService> _logger;


        public GoogleFormsService(HttpClient httpClient, ILogger<GoogleFormsService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<bool> SubmitToGoogleFormAsync(RegistrationRecord record)
        {
            try
            {
                // Google Forms submission can be implemented here if needed
                // For now, just log the submission
                _logger.LogInformation($"Google Form submission simulated for: {record.PhoneNumber}");
                await Task.Delay(100); // Simulate API call
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error submitting to Google Form");
                return false;
            }
        }
    }
}

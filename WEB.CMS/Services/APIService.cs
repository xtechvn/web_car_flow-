using Entities.Models;
using Entities.ViewModels;

using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Repositories.IRepositories;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Utilities;
using Utilities.Contants;
using WEB.CMS.Models;

namespace WEB.Adavigo.CMS.Service
{
    public class APIService
    {
        private readonly IConfiguration _configuration;
        private readonly IUserRepository _userRepository;
        public APIService(IConfiguration configuration, IUserRepository userRepository)
        {
            _configuration = configuration;
            _userRepository = userRepository;

        }
        
        
        public async Task<int> UpdateUser(UserViewModel model)
        {
            try
            {
                string md5_hash = "";
                if (model.Password != null && model.Password.Trim() != "")
                {
                    md5_hash = EncodeHelpers.MD5Hash(model.Password);
                }
                var old_company = model.OldCompanyType != null && model.OldCompanyType.Trim() != "" ? model.OldCompanyType.Split(",") : null;
                var deactive_company = new List<string>();
                if (old_company != null && old_company.Length > 0)
                {
                    foreach (var old in old_company)
                    {
                        if (!model.CompanyType.ToLower().Contains(old.ToLower()))
                        {
                            deactive_company.Add(old);
                        }
                    }
                }
                HttpClient httpClient = new HttpClient();
                var j_param = new UserAPIModel()
                {
                    Id = model.Id,//  Nếu là tạo mới truyền - 1,Nếu cập nhật thì sẽ truyền id của user cập nhật
                    UserName = model.UserName,
                    FullName = model.FullName,
                    Password = md5_hash ?? model.Password,
                    ResetPassword = md5_hash ?? model.Password,
                    Phone = model.Phone ?? "",
                    BirthDay = !string.IsNullOrEmpty(model.BirthDayPicker) ?
                                DateTime.ParseExact(model.BirthDayPicker, "dd/MM/yyyy", CultureInfo.InvariantCulture)
                              : model.BirthDay,
                    Gender = model.Gender,
                    Email = model.Email ?? "",
                    Avata = model.Avata ?? "",
                    Address = model.Address ?? "",
                    Status = model.Status, // 0: BÌnh thường
                    Note = model.Note ?? "",
                    CreatedBy = model.CreatedBy, // id của user nào tạo
                    ModifiedBy = model.ModifiedBy, // id của user nào update,
                    CompanyType = model.CompanyType, // loại công ty. 0: Travel | 1: Phú Quốc | 2: Đại Việt
                    CompanyDeactiveType = deactive_company.Count > 0 ? string.Join(",", deactive_company) : ""
                };


                var data_product = JsonConvert.SerializeObject(j_param);
                var token = CommonHelper.Encode(data_product, _configuration["DataBaseConfig:key_api:api_manual"]);


                var request = new FormUrlEncodedContent(new[]
                    {
                    new KeyValuePair<string, string>("token",token)
                });
                var url = "https://qc-login.adavigo.com" + "/api/authent/upsert_user.json";

                var response = await httpClient.PostAsync(url, request);


                if (response.IsSuccessStatusCode)
                {

                    var stringResult = response.Content.ReadAsStringAsync().Result;

                    var result = JsonConvert.DeserializeObject<UserAPIResponse>(stringResult);
                    model.Id = result.user_id;
                }

                return 1;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("apisever - UpdateUser:" + ex.ToString());
                return -1;
            }
        }
        
    
    }
}

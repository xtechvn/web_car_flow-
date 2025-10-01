using Entities.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Repositories.IRepositories;
using System.Security.Claims;
using Utilities;

namespace WEB.CMS.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserRepository _UserRepository;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _WebHostEnvironment;
        private readonly IRoleRepository _RoleRepository;
        public UserController(IUserRepository userRepository, IConfiguration configuration, IWebHostEnvironment hostEnvironment, IRoleRepository roleRepository)
        {
            _UserRepository = userRepository;
            _configuration = configuration;
            _WebHostEnvironment = hostEnvironment;
            _RoleRepository = roleRepository;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Search(string userName, int? status, int currentPage = 1, int pageSize = 20)
        {
            var model = new List<UserGridModel>();
            try
            {
                if (status < 0 || status > 2) status = null;
                if (userName != null && userName.Trim() != "") userName = CommonHelper.RemoveUnicode(userName);
                model = _UserRepository.GetPagingList(userName, status, currentPage, pageSize);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("Search - UserController: " + ex);
            }
            return PartialView(model);
        }
        [HttpPost]
        public async Task<IActionResult> UpSert(IFormFile imagefile, UserViewModel model)
        {
            try
            {
                string imageUrl = string.Empty;
                if (imagefile != null)
                {
                    string _FileName = Guid.NewGuid() + Path.GetExtension(imagefile.FileName);
                    string _UploadFolder = @"uploads/images";
                    string _UploadDirectory = Path.Combine(_WebHostEnvironment.WebRootPath, _UploadFolder);

                    if (!Directory.Exists(_UploadDirectory))
                    {
                        Directory.CreateDirectory(_UploadDirectory);
                    }

                    string filePath = Path.Combine(_UploadDirectory, _FileName);

                    if (!System.IO.File.Exists(filePath))
                    {
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await imagefile.CopyToAsync(fileStream);
                        }
                    }
                    model.Avata = "/" + _UploadFolder + "/" + _FileName;
                }

                int rs = 0;
                if (model.UserPositionId != null && model.UserPositionId > 0)
                {
                    var active_position = await _UserRepository.GetUserPositionsByID((int)model.UserPositionId);
                    if (active_position != null) model.Level = active_position.Rank;
                }
                if (model.CompanyType == null || model.CompanyType.Trim() == "")
                {
                    model.CompanyType = _configuration["CompanyType"];
                }
                var _UserLogin = 0;
                if (HttpContext.User.FindFirst(ClaimTypes.NameIdentifier) != null)
                {
                    _UserLogin = int.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
                }
                model.CreatedBy = _UserLogin;
                model.ModifiedBy = _UserLogin;
                if (model.Phone == null) model.Phone = "";
                if (model.Avata == null) model.Avata = "";
                if (model.Address == null) model.Address = "";
                if (model.NickName == null) model.NickName = "";

                //-- Update dbUser:

                var exists = await _UserRepository.GetById(model.Id);
                if (exists == null || exists.Id <= 0)
                {
                    rs = await _UserRepository.Create(model);

                }
                else
                {
                    rs = await _UserRepository.Update(model);

                }

                if (rs > 0)
                {
    
                    return new JsonResult(new
                    {
                        isSuccess = true,
                        message = "Cập nhật thành công"
                    });
                }
                else if (rs == -1)
                {
                    return new JsonResult(new
                    {
                        isSuccess = false,
                        message = "Tên đăng nhập hoặc email đã tồn tại"
                    });
                }
                else
                {
                    return new JsonResult(new
                    {
                        isSuccess = false,
                        message = "Cập nhật thất bại"
                    });
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("UpSert - UserController: " + ex);
                return new JsonResult(new
                {
                    isSuccess = false,
                    message = ex.Message.ToString()
                });
            }
        }
        [HttpPost]
        public async Task<IActionResult> GetDetail(int Id)
        {
            var model = new UserDataViewModel();
            try
            {
                model = await _UserRepository.GetUser(Id);
                ViewBag.RoleList = await _RoleRepository.GetAll();
                model.UserPositionName = model.UserPositionId != null && model.UserPositionId > 0 ? _UserRepository.GetUserPositionsByID((int)model.UserPositionId).Result.Name : "";
                ViewBag.IsMFAEnabled = Id;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetDetail - UserController: " + ex);
                ViewBag.IsMFAEnabled = false;
            }
            return PartialView(model);
        }
        public async Task<string> GetUserSuggestionList(string name)
        {
            try
            {
                var userlist = await _UserRepository.GetUserSuggesstion(name);
                var suggestionlist = userlist.Select(s => new
                {
                    id = s.Id,
                    name = s.UserName,
                    fullname = s.FullName,
                    email = s.Email
                }).ToList();
                return JsonConvert.SerializeObject(suggestionlist);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetUserSuggestionList - UserController: " + ex);
                return null;
            }
        }
    }
}

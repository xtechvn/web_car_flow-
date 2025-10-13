using Entities.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Repositories.IRepositories;
using Utilities;

namespace WEB.CMS.Controllers
{
    public class RoleController : Controller
    {
        private readonly IRoleRepository _RoleRepository;

        public RoleController(IRoleRepository roleRepository)
        {
            _RoleRepository = roleRepository;

        }

        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> AddOrUpdate(int Id)
        {
            var model = new RoleViewModel();
            if (Id != 0)
            {
                var roleEntity = await _RoleRepository.GetById(Id);
                model = new RoleViewModel()
                {
                    Id = roleEntity.Id,
                    Name = roleEntity.Name,
                    Description = roleEntity.Description,
                    Status = roleEntity.Status
                };
            }
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> UpSert(RoleViewModel model)
        {
            try
            {
                var rs = await _RoleRepository.Upsert(model);
                if (rs > 0)
                {
                    return new JsonResult(new
                    {
                        isSuccess = true,
                        message = "Cập nhật thành công"
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
                LogHelper.InsertLogTelegram("UpSert - RoleController: " + ex);
                return new JsonResult(new
                {
                    isSuccess = false,
                    message = "Cập nhật thất bại"
                });
            }
        }
        public async Task<string> GetRoleSuggestionList(string name)
        {
            try
            {
                var rolelist = await _RoleRepository.GetAll();

                if (!string.IsNullOrEmpty(name))
                {
                    rolelist = rolelist.Where(s => StringHelpers.ConvertStringToNoSymbol(s.Name.Trim().ToLower())
                                                   .Contains(StringHelpers.ConvertStringToNoSymbol(name.Trim().ToLower())))
                                                   .ToList();
                }
                var suggestionlist = rolelist.Take(5).Select(s => new
                {
                    id = s.Id,
                    name = s.Name
                }).ToList();

                return JsonConvert.SerializeObject(suggestionlist);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetRoleSuggestionList - RoleController: " + ex);
                return null;
            }
        }

        [HttpPost]
        public IActionResult Search(string roleName, string strUserId, int currentPage = 1, int pageSize = 8)
        {
            var model = new List<RoleDataModel>();
            try
            {
                model = _RoleRepository.GetPagingList(roleName, strUserId, currentPage, pageSize);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("Search - RoleController: " + ex);
            }
            return PartialView(model);
        }
    }
}

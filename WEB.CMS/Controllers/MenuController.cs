﻿using Entities.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Repositories.IRepositories;
using Repositories.Repositories;
using WEB.CMS.Customize;

namespace WEB.CMS.Controllers
{
    [CustomAuthorize]
    public class MenuController : Controller
    {
        private readonly IMenuRepository _MenuRepository;
        private readonly IConfiguration _configuration;

        public MenuController(IMenuRepository menuRepository, IConfiguration configuration)
        {
            _MenuRepository = menuRepository;
            _configuration = configuration;

        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Search(string name, string link)
        {
            var menus = await _MenuRepository.GetAll(name, link);
            return View(menus);
        }

        public async Task<IActionResult> AddOrUpdate(int id, int parent_id)
        {
            var model = new MenuUpsertViewModel
            {
                ParentId = parent_id,
                Status = 0
            };

            if (id > 0)
            {
                var menu = await _MenuRepository.GetById(id);
                model = new MenuUpsertViewModel
                {
                    Id = id,
                    ParentId = menu.ParentId,
                    MenuCode = menu.MenuCode,
                    Name = menu.Name,
                    Link = menu.Link,
                    Title = menu.Title,
                    Icon = menu.Icon,
                    Status = menu.Status
                };
            }
            return View(model);
        }


        public async Task<IActionResult> Permission(int id)
        {
            var model = new MenuPermissionModel
            {
                menu_id = id,
                permission_ids = new List<int>()
            };

            var permission_list = await _MenuRepository.GetSelectedPermissionList(id);
            if (permission_list != null && permission_list.Any())
            {
                model.permission_ids = permission_list;
            }

            ViewBag.Permissions = await _MenuRepository.GetListPermission();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> SavePermission(MenuPermissionModel model)
        {
            try
            {
                long result = await _MenuRepository.SavePermission(model);

                if (result > 0)
                {
                    return new JsonResult(new
                    {
                        isSuccess = true,
                        message = $"Cập nhật quyền cho menu thành công"
                    });
                }
                else
                {
                    return new JsonResult(new
                    {
                        isSuccess = false,
                        message = $"Cập nhật quyền cho menu thất bại"
                    });
                }
            }
            catch (Exception ex)
            {
                return new JsonResult(new
                {
                    isSuccess = false,
                    message = ex.Message
                });
            }
        }


        [HttpPost]
        public async Task<IActionResult> AddOrUpdate(MenuUpsertViewModel model)
        {
            try
            {
                long result = 0;
                string ActionName = string.Empty;
                if (model.Id > 0)
                {
                    ActionName = "Cập nhật";
                    result = await _MenuRepository.Update(model);
                }
                else
                {
                    ActionName = "Thêm mới";
                    result = await _MenuRepository.Create(model);
                }

                if (result > 0)
                {
                    return new JsonResult(new
                    {
                        isSuccess = true,
                        message = $"{ActionName} menu thành công"
                    });
                }
                else
                {
                    return new JsonResult(new
                    {
                        isSuccess = false,
                        message = $"{ActionName} menu thất bại"
                    });
                }
            }
            catch (Exception ex)
            {
                return new JsonResult(new
                {
                    isSuccess = false,
                    message = ex.Message
                });
            }
        }

        public async Task<IActionResult> ChangeStatus(int Id, int Status)
        {
            try
            {
                var result = await _MenuRepository.ChangeStatus(Id, Status);

                if (result > 0)
                {
                    return new JsonResult(new
                    {
                        isSuccess = true,
                        message = "Ẩn Menu thành công"
                    });
                }
                else
                {
                    return new JsonResult(new
                    {
                        isSuccess = false,
                        message = "Ẩn Menu thất bại"
                    });
                }
            }
            catch (Exception ex)
            {
                return new JsonResult(new
                {
                    isSuccess = false,
                    message = ex.Message
                });
            }
        }
    }
}

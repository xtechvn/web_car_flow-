using Microsoft.AspNetCore.Mvc;
using Repositories.IRepositories;

namespace WEB.CMS.Controllers
{
    public class DepartmentController : Controller
    {
        private readonly IDepartmentRepository _DepartmentRepository;
        private readonly IAllCodeRepository _allCodeRepository;
        public DepartmentController(IDepartmentRepository departmentRepository, IAllCodeRepository allCodeRepository)
        {
            _DepartmentRepository = departmentRepository;
            _allCodeRepository = allCodeRepository;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Search(string name)
        {
            var departments = await _DepartmentRepository.GetAll(name);
            return View(departments);
        }
    }
}

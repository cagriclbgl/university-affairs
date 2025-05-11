using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using UniversityAffairs.Models;

namespace UniversityAffairs.Controllers
{
    [Authorize(Roles = "DepartmentHead")]
    public class UserManagementController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserManagementController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.Roles = new[] { "Instructor", "Secretary", "DepartmentHead" };
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(string fullName, string username, string password, string role)
        {
            var user = new ApplicationUser
            {
                UserName = username,
                FullName = fullName
            };

            var result = await _userManager.CreateAsync(user, password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, role);
                ViewBag.Success = "Kullanıcı başarıyla oluşturuldu.";
            }
            else
            {
                ViewBag.Error = string.Join(" | ", result.Errors.Select(e => e.Description));
            }

            ViewBag.Roles = new[] { "Instructor", "Secretary", "DepartmentHead" };
            return View();
        }
    }
}

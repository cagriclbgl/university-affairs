using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using UniversityAffairs.Models;

namespace UniversityAffairs.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public AccountController(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            var result = await _signInManager.PasswordSignInAsync(username, password, false, false);
            if (result.Succeeded)
            {
                var user = await _userManager.FindByNameAsync(username);
                var roles = await _userManager.GetRolesAsync(user);
                var role = roles.FirstOrDefault();

                return role switch
                {
                    "DepartmentHead" => RedirectToAction("Index", "Admin"),
                    "Secretary" => RedirectToAction("Index", "Secretary"),
                    "Instructor" => RedirectToAction("WeeklySchedule", "Instructor"),
                    _ => RedirectToAction("Login")
                };
            }

            ViewBag.Error = "Giriş başarısız. Kullanıcı adı veya şifre yanlış.";
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login");
        }

        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}

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
            var user = await _userManager.FindByNameAsync(username);
            if (user != null)
            {
                var result = await _signInManager.PasswordSignInAsync(user, password, false, false);
                if (result.Succeeded)
                {
                    var roles = await _userManager.GetRolesAsync(user);

                    if (roles.Contains("DepartmentHead"))
                        return RedirectToAction("Panel", "Admin");

                    if (roles.Contains("Secretary"))
                        return RedirectToAction("Index", "Secretary");

                    if (roles.Contains("Instructor"))
                        return RedirectToAction("WeeklySchedule", "Instructor");

                    // Varsayılan fallback
                    return RedirectToAction("Index", "Home");
                }
            }

            ViewBag.Error = "Geçersiz kullanıcı adı veya şifre.";
            return View();
        }




        [HttpGet]
        public async Task<IActionResult> CreateInstructorUser()
        {
            var email = "instructor1@demo.com";
            var password = "Instructor123!";
            var fullName = "Ali Öğretim"; // ← bunu da veriyoruz

            var user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                FullName = fullName
            };

            var result = await _userManager.CreateAsync(user, password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "Instructor");
                return Content("Instructor kullanıcı başarıyla oluşturuldu.");
            }

            return Content("Kullanıcı oluşturulamadı.");
        }


        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }

        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}

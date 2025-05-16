using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace UniversityAffairs.Controllers
{
    [Authorize(Roles = "DepartmentHead")]
    public class AdminController : Controller
    {
        // /Admin/Index yönlendirmelerini karşılar
        public IActionResult Index()
        {
            // Direkt Panel metoduna yönlendir
            return RedirectToAction("Panel");
        }

        public IActionResult Panel()
        {
            return View("AdminPanel");
        }
    }
}

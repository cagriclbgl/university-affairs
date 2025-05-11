using Microsoft.AspNetCore.Mvc;

namespace UniversityAffairs.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Panel()
        {
            return View("AdminPanel");
        }
    }
}

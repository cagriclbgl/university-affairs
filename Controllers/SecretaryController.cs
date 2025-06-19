using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace UniversityAffairs.Controllers
{
    [Authorize(Roles = "DepartmentHead,Secretary")]
    public class SecretaryController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using UniversityAffairs.Data;
using UniversityAffairs.Models;

namespace UniversityAffairs.Controllers
{
    [Authorize(Roles = "Instructor")]
    public class InstructorScheduleController : Controller
    {
        private readonly UniversityDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public InstructorScheduleController(UniversityDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> MySchedule()
        {
            var user = await _userManager.GetUserAsync(User);

            var schedules = await _context.LessonSchedules
                .Include(ls => ls.Lesson)
                .Include(ls => ls.Classroom)
                .Include(ls => ls.Grade)
                .Include(ls => ls.Term)
                .Include(ls => ls.Instructor)
                .Where(ls => ls.Instructor.Email == user.Email)
                .ToListAsync();

            return View(schedules);
        }
    }
}

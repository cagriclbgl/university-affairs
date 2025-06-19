using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniversityAffairs.Data;
using UniversityAffairs.Models;
using UniversityAffairs.Services;

namespace UniversityAffairs.Controllers
{
    [Authorize]
    public class SeatingPlanController : Controller
    {
        private readonly UniversityDbContext _context;
        private readonly SeatingPlanService _seatingPlanService;
        private readonly UserManager<ApplicationUser> _userManager;

        public SeatingPlanController(UniversityDbContext context, SeatingPlanService seatingPlanService, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _seatingPlanService = seatingPlanService;
            _userManager = userManager;
        }

        [Authorize(Roles = "DepartmentHead,Secretary")]
        public IActionResult Generate(int examScheduleId)
        {
            List<string> fakeStudentNumbers = Enumerable.Range(1, 50)
                .Select(i => $"2023{i:D3}")
                .ToList();

            var seatingPlan = _seatingPlanService.GenerateSeatingPlan(examScheduleId, fakeStudentNumbers);
            return View("Index", seatingPlan);
        }

        // 🔹 Sınavlara göre filtrelenmiş Manage görünümü
        [HttpGet]
        [Authorize(Roles = "DepartmentHead,Secretary")]
        public async Task<IActionResult> Manage()
        {
            ViewBag.Exams = await _context.ExamSchedules
                .Include(e => e.Lesson)
                .ToListAsync();

            return View("Manage", new List<SeatingPlan>());
        }

        [HttpPost]
        [ActionName("Manage")] // Route adı yine "Manage" olur
        [Authorize(Roles = "DepartmentHead,Secretary")]
        public async Task<IActionResult> ManagePost(int examScheduleId)
        {
            ViewBag.Exams = await _context.ExamSchedules
                .Include(e => e.Lesson)
                .ToListAsync();

            var seatingPlans = await _context.SeatingPlans
                .Include(p => p.ExamSchedule).ThenInclude(e => e.Lesson)
                .Include(p => p.ExamSchedule).ThenInclude(e => e.Instructor)
                .Where(p => p.ExamScheduleId == examScheduleId)
                .ToListAsync();

            return View("Manage", seatingPlans);
        }

        [HttpPost]
        [Authorize(Roles = "DepartmentHead,Secretary")]
        public async Task<IActionResult> DeleteAll(int examScheduleId)
        {
            var plansToDelete = await _context.SeatingPlans
                .Where(p => p.ExamScheduleId == examScheduleId)
                .ToListAsync();

            if (!plansToDelete.Any())
            {
                TempData["Error"] = "Bu ID ile kayıtlı oturma planı bulunamadı.";
                return RedirectToAction("Manage");
            }

            _context.SeatingPlans.RemoveRange(plansToDelete);
            await _context.SaveChangesAsync();

            TempData["Message"] = "Oturma planı başarıyla silindi.";
            return RedirectToAction("Manage");
        }

        // 🔹 Öğretim Elemanları kendi planlarını görür
        [Authorize(Roles = "Instructor")]
        public async Task<IActionResult> MySeatingPlans()
        {
            var user = await _userManager.GetUserAsync(User);
            var instructor = await _context.Instructors.FirstOrDefaultAsync(i => i.Email == user.Email);

            var myPlans = await _context.SeatingPlans
                .Include(p => p.ExamSchedule)
                    .ThenInclude(e => e.Lesson)
                .Include(p => p.ExamSchedule.Instructor)
                .Where(p => p.ExamSchedule.InstructorId == instructor.Id)
                .ToListAsync();

            return View("MyPlans", myPlans);
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniversityAffairs.Data;
using UniversityAffairs.Models;
using UniversityAffairs.Services;

namespace UniversityAffairs.Controllers
{
    public class SeatingPlanController : Controller
    {
        private readonly UniversityDbContext _context;
        private readonly SeatingPlanService _seatingPlanService;

        public SeatingPlanController(UniversityDbContext context, SeatingPlanService seatingPlanService)
        {
            _context = context;
            _seatingPlanService = seatingPlanService;
        }

        // Örnek: /SeatingPlan/Generate?examScheduleId=1
        public IActionResult Generate(int examScheduleId)
        {
            // Burada şimdilik örnek öğrenci numaralarıyla simülasyon yapıyoruz.
            List<string> fakeStudentNumbers = Enumerable.Range(1, 50)
                .Select(i => $"2023{i:D3}")
                .ToList();

            var seatingPlan = _seatingPlanService.GenerateSeatingPlan(examScheduleId, fakeStudentNumbers);
            return View("Index", seatingPlan);
        }
    }
}

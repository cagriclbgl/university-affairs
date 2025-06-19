using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UniversityAffairs.Data;
using UniversityAffairs.Models;

namespace UniversityAffairs.Controllers
{
    [Authorize(Roles = "DepartmentHead,Secretary,Instructor")]
    public class InstructorController : Controller
    {
        private readonly UniversityDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public InstructorController(UniversityDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // 🟦 Öğretim Elemanı Paneli
        [Authorize(Roles = "Instructor,DepartmentHead")]
        public IActionResult Index()
        {
            return View(); // Artık Instructor Panel sayfasını döndürüyoruz
        }


        // 🟪 Haftalık Ders Programı
        [Authorize(Roles = "Instructor,DepartmentHead,Secretary")]
        public async Task<IActionResult> WeeklySchedule()
        {
            var user = await _userManager.GetUserAsync(User);

            // Instructor tablosundaki kullanıcıya karşılık gelen InstructorId'yi bul
            var instructor = await _context.Instructors.FirstOrDefaultAsync(i => i.Email == user.Email);

            if (instructor == null)
            {
                return NotFound("Instructor kaydı bulunamadı.");
            }

            // InstructorId ile filtrele
            var schedules = await _context.LessonSchedules
                .Include(s => s.Lesson)
                .Include(s => s.Classroom)
                .Include(s => s.Grade)
                .Include(s => s.Term)
                .Where(s => s.InstructorId == instructor.Id)
                .ToListAsync();

            return View("~/Views/LessonSchedule/WeeklySchedule.cshtml", schedules);
        }


        // 🟨 Sınavlarım
        [Authorize(Roles = "Instructor,DepartmentHead")]
        public async Task<IActionResult> MyExams()
        {
            var user = await _userManager.GetUserAsync(User);

            var exams = await _context.ExamSchedules
                .Include(e => e.Lesson)
                .Include(e => e.Classroom)
                .Include(e => e.Grade)
                .Include(e => e.Term)
                .Include(e => e.Instructor)
                .Where(e => e.Instructor.Email == user.Email)
                .ToListAsync();

            return View(exams);
        }

        [Authorize(Roles = "Instructor,DepartmentHead")]
        public async Task<IActionResult> MySeatingPlans()
        {
            var user = await _userManager.GetUserAsync(User);

            var instructor = await _context.Instructors
                .FirstOrDefaultAsync(i => i.Email == user.Email);

            if (instructor == null)
            {
                return NotFound("Instructor kaydı bulunamadı.");
            }

            var plans = await _context.SeatingPlans
                .Include(p => p.ExamSchedule)
                    .ThenInclude(e => e.Lesson)
                .Include(p => p.ExamSchedule)
                    .ThenInclude(e => e.Instructor)
                .Where(p => p.ExamSchedule.InstructorId == instructor.Id)
                .ToListAsync();

            return View("~/Views/SeatingPlan/MyPlans.cshtml", plans);
        }


        // 🔧 CRUD işlemleri

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var instructor = await _context.Instructors.FirstOrDefaultAsync(m => m.Id == id);
            if (instructor == null) return NotFound();

            return View(instructor);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,FullName,Title,Email")] Instructor instructor)
        {
            if (!ModelState.IsValid)
            {
                Console.WriteLine("Model geçersiz!");
                return View(instructor);
            }

            Console.WriteLine("Model geçerli, kayıt ekleniyor...");
            _context.Add(instructor);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var instructor = await _context.Instructors.FindAsync(id);
            if (instructor == null) return NotFound();

            return View(instructor);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FullName,Title,Email")] Instructor instructor)
        {
            if (id != instructor.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(instructor);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Instructors.Any(e => e.Id == instructor.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(instructor);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var instructor = await _context.Instructors.FirstOrDefaultAsync(m => m.Id == id);
            if (instructor == null) return NotFound();

            return View(instructor);
        }

        [Authorize(Roles = "DepartmentHead,Secretary")]
        public async Task<IActionResult> ListAll()
        {
            var instructors = await _context.Instructors.ToListAsync();
            return View("Manage", instructors); // yeni View: Views/Instructor/Manage.cshtml
        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var instructor = await _context.Instructors.FindAsync(id);
            if (instructor != null)
            {
                _context.Instructors.Remove(instructor);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}

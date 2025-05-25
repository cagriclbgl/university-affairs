using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using UniversityAffairs.Data;
using UniversityAffairs.Models;

namespace UniversityAffairs.Controllers
{
    [Authorize(Roles = "DepartmentHead,Secretary")]
    public class ExamScheduleController : Controller
    {
        private readonly UniversityDbContext _context;

        public ExamScheduleController(UniversityDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var exams = _context.ExamSchedules
                .Include(e => e.Lesson)
                .Include(e => e.Instructor)
                .Include(e => e.Classroom)
                .Include(e => e.Grade)
                .Include(e => e.Term);

            return View(await exams.ToListAsync());
        }

        public IActionResult Create()
        {
            LoadDropdowns();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ExamSchedule examSchedule)
        {
            if (ModelState.IsValid)
            {
                _context.Add(examSchedule);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            foreach (var key in ModelState.Keys)
            {
                var state = ModelState[key];
                foreach (var error in state.Errors)
                {
                    Console.WriteLine($"Model hatası → {key}: {error.ErrorMessage}");
                }
            }

            LoadDropdowns();
            return View(examSchedule);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var exam = await _context.ExamSchedules.FindAsync(id);
            if (exam == null) return NotFound();

            LoadDropdowns();
            return View(exam);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ExamSchedule examSchedule)
        {
            if (id != examSchedule.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(examSchedule);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.ExamSchedules.Any(e => e.Id == examSchedule.Id))
                        return NotFound();
                    else throw;
                }
            }

            foreach (var key in ModelState.Keys)
            {
                var state = ModelState[key];
                foreach (var error in state.Errors)
                {
                    Console.WriteLine($"Model hatası → {key}: {error.ErrorMessage}");
                }
            }

            LoadDropdowns();
            return View(examSchedule);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var exam = await _context.ExamSchedules
                .Include(e => e.Lesson)
                .Include(e => e.Instructor)
                .Include(e => e.Classroom)
                .Include(e => e.Grade)
                .Include(e => e.Term)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (exam == null) return NotFound();

            return View(exam);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var exam = await _context.ExamSchedules.FindAsync(id);
            if (exam != null)
            {
                _context.ExamSchedules.Remove(exam);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        // 🔄 Tekrarlanan ViewData ayarlarını yöneten metot
        private void LoadDropdowns()
        {
            ViewData["LessonId"] = new SelectList(_context.Lessons, "Id", "LessonName");
            ViewData["InstructorId"] = new SelectList(_context.Instructors, "Id", "FullName");
            ViewData["ClassroomId"] = new SelectList(_context.Classrooms, "Id", "RoomName");
            ViewData["GradeId"] = new SelectList(_context.Grades, "Id", "Name");
            ViewData["TermId"] = new SelectList(_context.Terms, "Id", "Name");
        }
    }
}

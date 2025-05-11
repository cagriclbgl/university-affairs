using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using UniversityAffairs.Data;
using UniversityAffairs.Models;

namespace UniversityAffairs.Controllers
{
    public class LessonScheduleController : Controller
    {
        private readonly UniversityDbContext _context;

        public LessonScheduleController(UniversityDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int? gradeId, int? termId)
        {
            var query = _context.LessonSchedules
                .Include(x => x.Lesson)
                .Include(x => x.Instructor)
                .Include(x => x.Classroom)
                .Include(x => x.Grade)
                .Include(x => x.Term)
                .AsQueryable();

            if (gradeId.HasValue)
                query = query.Where(x => x.GradeId == gradeId.Value);

            if (termId.HasValue)
                query = query.Where(x => x.TermId == termId.Value);

            ViewData["GradeId"] = new SelectList(_context.Grades, "Id", "Name");
            ViewData["TermId"] = new SelectList(_context.Terms, "Id", "Name");

            return View(await query.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var lessonSchedule = await _context.LessonSchedules
                .Include(l => l.Classroom)
                .Include(l => l.Instructor)
                .Include(l => l.Lesson)
                .Include(l => l.Grade)
                .Include(l => l.Term)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (lessonSchedule == null) return NotFound();

            return View(lessonSchedule);
        }

        public async Task<IActionResult> WeeklySchedule(int? gradeId, int? termId)
        {
            var query = _context.LessonSchedules
                .Include(x => x.Lesson)
                .Include(x => x.Instructor)
                .Include(x => x.Classroom)
                .Include(x => x.Grade)
                .Include(x => x.Term)
                .AsQueryable();

            if (gradeId.HasValue)
                query = query.Where(x => x.GradeId == gradeId.Value);

            if (termId.HasValue)
                query = query.Where(x => x.TermId == termId.Value);

            // Filtrelemede hata varsa görünmez, ViewData her zaman dolu olmalı:
            ViewData["GradeId"] = new SelectList(_context.Grades, "Id", "Name", gradeId);
            ViewData["TermId"] = new SelectList(_context.Terms, "Id", "Name", termId);

            var schedules = await query.ToListAsync();
            return View(schedules);
        }


        public IActionResult Create()
        {
            PopulateSelectLists();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Day,StartTime,EndTime,LessonId,InstructorId,ClassroomId,GradeId,TermId")] LessonSchedule lessonSchedule)
        {
            if (lessonSchedule.EndTime <= lessonSchedule.StartTime)
                ModelState.AddModelError("", "End time must be after start time.");

            if (lessonSchedule.StartTime.Hours < 8 || lessonSchedule.EndTime.Hours > 17 ||
                (lessonSchedule.EndTime.Hours == 17 && lessonSchedule.EndTime.Minutes > 0))
                ModelState.AddModelError("", "Lessons must be scheduled between 08:00 and 17:00.");

            bool isClassroomConflict = await _context.LessonSchedules.AnyAsync(s =>
                s.Day == lessonSchedule.Day &&
                s.ClassroomId == lessonSchedule.ClassroomId &&
                ((lessonSchedule.StartTime >= s.StartTime && lessonSchedule.StartTime < s.EndTime) ||
                (lessonSchedule.EndTime > s.StartTime && lessonSchedule.EndTime <= s.EndTime) ||
                (lessonSchedule.StartTime <= s.StartTime && lessonSchedule.EndTime >= s.EndTime)));

            if (isClassroomConflict)
                ModelState.AddModelError("", "This classroom is already occupied during the selected time.");

            bool isInstructorConflict = await _context.LessonSchedules.AnyAsync(s =>
                s.Day == lessonSchedule.Day &&
                s.InstructorId == lessonSchedule.InstructorId &&
                ((lessonSchedule.StartTime >= s.StartTime && lessonSchedule.StartTime < s.EndTime) ||
                (lessonSchedule.EndTime > s.StartTime && lessonSchedule.EndTime <= s.EndTime) ||
                (lessonSchedule.StartTime <= s.StartTime && lessonSchedule.EndTime >= s.EndTime)));

            if (isInstructorConflict)
                ModelState.AddModelError("", "This instructor is already teaching another lesson at the selected time.");

            bool isExactSameLessonScheduled = await _context.LessonSchedules.AnyAsync(s =>
                s.LessonId == lessonSchedule.LessonId &&
                s.GradeId == lessonSchedule.GradeId &&
                s.TermId == lessonSchedule.TermId &&
                s.Day == lessonSchedule.Day &&
                s.StartTime == lessonSchedule.StartTime &&
                s.EndTime == lessonSchedule.EndTime &&
                s.InstructorId == lessonSchedule.InstructorId &&
                s.ClassroomId == lessonSchedule.ClassroomId);

            if (isExactSameLessonScheduled)
                ModelState.AddModelError("", "This exact lesson is already scheduled at the same time, with the same instructor and classroom.");

            if (ModelState.IsValid)
            {
                _context.Add(lessonSchedule);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            PopulateSelectLists(lessonSchedule);
            return View(lessonSchedule);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var lessonSchedule = await _context.LessonSchedules.FindAsync(id);
            if (lessonSchedule == null) return NotFound();

            PopulateSelectLists(lessonSchedule);
            return View(lessonSchedule);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Day,StartTime,EndTime,LessonId,InstructorId,ClassroomId,GradeId,TermId")] LessonSchedule lessonSchedule)
        {
            if (id != lessonSchedule.Id) return NotFound();

            if (lessonSchedule.EndTime <= lessonSchedule.StartTime)
                ModelState.AddModelError("", "End time must be after start time.");

            if (lessonSchedule.StartTime.Hours < 8 || lessonSchedule.EndTime.Hours > 17 ||
                (lessonSchedule.EndTime.Hours == 17 && lessonSchedule.EndTime.Minutes > 0))
                ModelState.AddModelError("", "Lessons must be scheduled between 08:00 and 17:00.");

            bool isClassroomConflict = await _context.LessonSchedules.AnyAsync(s =>
                s.Id != lessonSchedule.Id &&
                s.Day == lessonSchedule.Day &&
                s.ClassroomId == lessonSchedule.ClassroomId &&
                ((lessonSchedule.StartTime >= s.StartTime && lessonSchedule.StartTime < s.EndTime) ||
                (lessonSchedule.EndTime > s.StartTime && lessonSchedule.EndTime <= s.EndTime) ||
                (lessonSchedule.StartTime <= s.StartTime && lessonSchedule.EndTime >= s.EndTime)));

            if (isClassroomConflict)
                ModelState.AddModelError("", "This classroom is already occupied during the selected time.");

            bool isInstructorConflict = await _context.LessonSchedules.AnyAsync(s =>
                s.Id != lessonSchedule.Id &&
                s.Day == lessonSchedule.Day &&
                s.InstructorId == lessonSchedule.InstructorId &&
                ((lessonSchedule.StartTime >= s.StartTime && lessonSchedule.StartTime < s.EndTime) ||
                (lessonSchedule.EndTime > s.StartTime && lessonSchedule.EndTime <= s.EndTime) ||
                (lessonSchedule.StartTime <= s.StartTime && lessonSchedule.EndTime >= s.EndTime)));

            if (isInstructorConflict)
                ModelState.AddModelError("", "This instructor is already teaching another lesson at the selected time.");

            bool isExactSameLessonScheduled = await _context.LessonSchedules.AnyAsync(s =>
                s.Id != lessonSchedule.Id &&
                s.LessonId == lessonSchedule.LessonId &&
                s.GradeId == lessonSchedule.GradeId &&
                s.TermId == lessonSchedule.TermId &&
                s.Day == lessonSchedule.Day &&
                s.StartTime == lessonSchedule.StartTime &&
                s.EndTime == lessonSchedule.EndTime &&
                s.InstructorId == lessonSchedule.InstructorId &&
                s.ClassroomId == lessonSchedule.ClassroomId);

            if (isExactSameLessonScheduled)
                ModelState.AddModelError("", "This exact lesson is already scheduled at the same time, with the same instructor and classroom.");

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(lessonSchedule);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LessonScheduleExists(lessonSchedule.Id)) return NotFound();
                    else throw;
                }
            }

            PopulateSelectLists(lessonSchedule);
            return View(lessonSchedule);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var lessonSchedule = await _context.LessonSchedules
                .Include(l => l.Classroom)
                .Include(l => l.Instructor)
                .Include(l => l.Lesson)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (lessonSchedule == null) return NotFound();

            return View(lessonSchedule);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var lessonSchedule = await _context.LessonSchedules.FindAsync(id);
            if (lessonSchedule != null)
            {
                _context.LessonSchedules.Remove(lessonSchedule);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool LessonScheduleExists(int id)
        {
            return _context.LessonSchedules.Any(e => e.Id == id);
        }

        private void PopulateSelectLists(LessonSchedule? schedule = null)
        {
            ViewData["ClassroomId"] = new SelectList(_context.Classrooms, "Id", "RoomName", schedule?.ClassroomId);
            ViewData["InstructorId"] = new SelectList(_context.Instructors, "Id", "FullName", schedule?.InstructorId);
            ViewData["LessonId"] = new SelectList(_context.Lessons, "Id", "LessonName", schedule?.LessonId);
            ViewData["GradeId"] = new SelectList(_context.Grades, "Id", "Name", schedule?.GradeId);
            ViewData["TermId"] = new SelectList(_context.Terms, "Id", "Name", schedule?.TermId);
        }
    }
}
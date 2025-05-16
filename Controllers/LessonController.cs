using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using UniversityAffairs.Data;
using UniversityAffairs.Models;

namespace UniversityAffairs.Controllers
{
    [Authorize(Roles = "DepartmentHead,Secretary")]
    public class LessonController : Controller
    {
        private readonly UniversityDbContext _context;

        public LessonController(UniversityDbContext context)
        {
            _context = context;
        }

        // GET: Lesson
        public async Task<IActionResult> Index()
        {
            return View(await _context.Lessons.ToListAsync());
        }

        // GET: Lesson/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lesson = await _context.Lessons
                .FirstOrDefaultAsync(m => m.Id == id);
            if (lesson == null)
            {
                return NotFound();
            }

            return View(lesson);
        }

        // GET: Lesson/Create
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,LessonName,LessonCode,Credit")] Lesson lesson)
        {
            // ✅ Kod benzersiz mi?
            bool isDuplicateCode = await _context.Lessons.AnyAsync(l => l.LessonCode == lesson.LessonCode);
            if (isDuplicateCode)
            {
                ModelState.AddModelError("LessonCode", "This lesson code is already in use.");
            }

            // ✅ İsim benzersiz mi?
            bool isDuplicateName = await _context.Lessons.AnyAsync(l => l.LessonName == lesson.LessonName);
            if (isDuplicateName)
            {
                ModelState.AddModelError("LessonName", "This lesson name is already in use.");
            }

            if (ModelState.IsValid)
            {
                _context.Add(lesson);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(lesson);
        }



        // GET: Lesson/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lesson = await _context.Lessons.FindAsync(id);
            if (lesson == null)
            {
                return NotFound();
            }
            return View(lesson);
        }

        // POST: Lesson/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,LessonName,LessonCode,Credit")] Lesson lesson)
        {
            if (id != lesson.Id)
            {
                return NotFound();
            }

            // ✅ Kod benzersiz mi (kendisinden başka)?
            bool isDuplicateCode = await _context.Lessons.AnyAsync(l => l.Id != lesson.Id && l.LessonCode == lesson.LessonCode);
            if (isDuplicateCode)
            {
                ModelState.AddModelError("LessonCode", "This lesson code is already in use.");
            }

            // ✅ İsim benzersiz mi (kendisinden başka)?
            bool isDuplicateName = await _context.Lessons.AnyAsync(l => l.Id != lesson.Id && l.LessonName == lesson.LessonName);
            if (isDuplicateName)
            {
                ModelState.AddModelError("LessonName", "This lesson name is already in use.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(lesson);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Lessons.Any(e => e.Id == lesson.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            return View(lesson);
        }



        // GET: Lesson/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lesson = await _context.Lessons
                .FirstOrDefaultAsync(m => m.Id == id);
            if (lesson == null)
            {
                return NotFound();
            }

            return View(lesson);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var lesson = await _context.Lessons.FindAsync(id);
            if (lesson != null)
            {
                _context.Lessons.Remove(lesson);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }


        private bool LessonExists(int id)
        {
            return _context.Lessons.Any(e => e.Id == id);
        }
    }
}

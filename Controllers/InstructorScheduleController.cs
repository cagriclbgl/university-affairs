using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using UniversityAffairs.Data;
using UniversityAffairs.Models;
using DinkToPdf;
using DinkToPdf.Contracts;

namespace UniversityAffairs.Controllers
{
    [Authorize(Roles = "Instructor")]
    public class InstructorScheduleController : Controller
    {
        private readonly UniversityDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConverter _converter;

        public InstructorScheduleController(UniversityDbContext context, UserManager<ApplicationUser> userManager, IConverter converter)
        {
            _context = context;
            _userManager = userManager;
            _converter = converter;
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



        [Authorize(Roles = "Instructor")]
        [HttpPost]
        public async Task<IActionResult> GeneratePdf()
        {
            var user = await _userManager.GetUserAsync(User);

            var schedules = await _context.LessonSchedules
                .Include(x => x.Lesson)
                .Include(x => x.Classroom)
                .Include(x => x.Grade)
                .Include(x => x.Term)
                .Include(x => x.Instructor)
                .Where(x => x.Instructor.Email == user.Email)
                .ToListAsync();

            
            string html = @"
    <html>
    <head>
        <meta charset='UTF-8'>
        <style>
            body { font-family: 'DejaVu Sans', sans-serif; font-size: 12px; }
            h2 { text-align: center; }
            table { width: 100%; border-collapse: collapse; margin-top: 20px; }
            th, td { border: 1px solid #000; padding: 5px; text-align: center; }
            th { background-color: #eee; }
        </style>
    </head>
    <body>
        <h2>Ders Programı</h2>
        <table>
            <tr>
                <th>Ders</th>
                <th>Gün</th>
                <th>Başlangıç</th>
                <th>Bitiş</th>
                <th>Sınıf</th>
                <th>Sınıf Düzeyi</th>
                <th>Dönem</th>
            </tr>";

            foreach (var item in schedules)
            {
                html += $@"
        <tr>
            <td>{item.Lesson.LessonName}</td>
            <td>{item.Day}</td>
            <td>{item.StartTime.ToString(@"hh\:mm")}</td>
            <td>{item.EndTime.ToString(@"hh\:mm")}</td>
            <td>{item.Classroom.RoomName}</td>
            <td>{item.Grade.Name}</td>
            <td>{item.Term.Name}</td>
        </tr>";
            }


            html += @"
        </table>
    </body>
    </html>";

            var doc = new HtmlToPdfDocument()
            {
                GlobalSettings = {
            PaperSize = PaperKind.A4,
            Orientation = Orientation.Portrait
        },
                Objects = {
            new ObjectSettings()
            {
                HtmlContent = html
            }
        }
            };

            var pdf = _converter.Convert(doc);
            return File(pdf, "application/pdf", "KapiIsimligi.pdf");
        }

    }
}

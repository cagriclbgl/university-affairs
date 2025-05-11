using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using UniversityAffairs.Models;

namespace UniversityAffairs.Data
{
    public class UniversityDbContext : IdentityDbContext<ApplicationUser>
    {
        public UniversityDbContext(DbContextOptions<UniversityDbContext> options)
            : base(options)
        {
        }

        public DbSet<Lesson> Lessons { get; set; }
        public DbSet<Instructor> Instructors { get; set; }
        public DbSet<Classroom> Classrooms { get; set; }
        public DbSet<LessonSchedule> LessonSchedules { get; set; }
        public DbSet<Grade> Grades { get; set; }
        public DbSet<Term> Terms { get; set; }
    }
}
 
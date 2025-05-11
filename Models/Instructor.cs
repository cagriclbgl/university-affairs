using System.ComponentModel.DataAnnotations;

namespace UniversityAffairs.Models
{
    public class Instructor
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Full Name")]
        public string FullName { get; set; }

        [Display(Name = "Title")]
        public string? Title { get; set; }

        [EmailAddress]
        [Display(Name = "Email")]
        public string? Email { get; set; }

        // Relationships
        public ICollection<LessonSchedule>? LessonSchedules { get; set; }
    }
}

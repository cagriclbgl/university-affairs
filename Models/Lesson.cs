using System.ComponentModel.DataAnnotations;

namespace UniversityAffairs.Models
{
    public class Lesson
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Lesson Name")]
        public string LessonName { get; set; }

        [Required]
        [Display(Name = "Lesson Code")]
        public string LessonCode { get; set; }

        [Display(Name = "Credit")]
        public int Credit { get; set; }

        // Relationships
        public ICollection<LessonSchedule>? LessonSchedules { get; set; }
    }
}

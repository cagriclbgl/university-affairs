using System.ComponentModel.DataAnnotations;

namespace UniversityAffairs.Models
{
    public class Classroom
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Room Name")]
        public string RoomName { get; set; }

        [Display(Name = "Capacity")]
        public int Capacity { get; set; }

        // Relationships
        public ICollection<LessonSchedule>? LessonSchedules { get; set; }
    }
}

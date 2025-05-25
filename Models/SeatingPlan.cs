using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UniversityAffairs.Models
{
    public class SeatingPlan
    {
        public int Id { get; set; }

        [Required]
        public int ExamScheduleId { get; set; }

        [ForeignKey("ExamScheduleId")]
        public ExamSchedule ExamSchedule { get; set; }

        [Required]
        public string StudentNumber { get; set; }

        [Required]
        public int Row { get; set; }

        [Required]
        public int Column { get; set; }

        [NotMapped]
        public string Block => Column switch
        {
            >= 0 and <= 3 => "Left",
            >= 4 and <= 6 => "Middle",
            >= 7 and <= 10 => "Right",
            _ => "Unknown"
        };
    }
}

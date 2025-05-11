using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UniversityAffairs.Models
{
    public class LessonSchedule
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Day is required.")]
        [Display(Name = "Day")]
        public string Day { get; set; }

        [Required(ErrorMessage = "Start time is required.")]
        [Display(Name = "Start Time")]
        [DataType(DataType.Time)]
        public TimeSpan StartTime { get; set; }

        [Required(ErrorMessage = "End time is required.")]
        [Display(Name = "End Time")]
        [DataType(DataType.Time)]
        public TimeSpan EndTime { get; set; }

        // Foreign Keys
        [Required(ErrorMessage = "Lesson selection is required.")]
        [Display(Name = "Lesson")]
        public int LessonId { get; set; }

        [ForeignKey("LessonId")]
        public Lesson? Lesson { get; set; }

        [Required(ErrorMessage = "Instructor selection is required.")]
        [Display(Name = "Instructor")]
        public int InstructorId { get; set; }

        [ForeignKey("InstructorId")]
        public Instructor? Instructor { get; set; }

        [Required(ErrorMessage = "Classroom selection is required.")]
        [Display(Name = "Classroom")]
        public int ClassroomId { get; set; }

        [ForeignKey("ClassroomId")]
        public Classroom? Classroom { get; set; }

        [Required]
        public int GradeId { get; set; }

        [ValidateNever]
        public Grade Grade { get; set; }

        [Required]
        public int TermId { get; set; }

        [ValidateNever]
        public Term Term { get; set; }


    }
}

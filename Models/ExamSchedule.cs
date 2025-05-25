using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UniversityAffairs.Models
{
    public class ExamSchedule
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Sınav tarihi zorunludur.")]
        [Display(Name = "Sınav Tarihi")]
        [DataType(DataType.Date)]
        public DateTime ExamDate { get; set; }

        [Required(ErrorMessage = "Başlangıç saati zorunludur.")]
        [Display(Name = "Başlangıç Saati")]
        [DataType(DataType.Time)]
        public TimeSpan StartTime { get; set; }

        [Required(ErrorMessage = "Bitiş saati zorunludur.")]
        [Display(Name = "Bitiş Saati")]
        [DataType(DataType.Time)]
        public TimeSpan EndTime { get; set; }

        // Foreign key: Ders
        [Required(ErrorMessage = "Ders seçimi zorunludur.")]
        public int LessonId { get; set; }

        [ForeignKey("LessonId")]
        public Lesson? Lesson { get; set; }

        // Foreign key: Öğretim Elemanı
        [Required(ErrorMessage = "Öğretim elemanı seçimi zorunludur.")]
        public int InstructorId { get; set; }

        [ForeignKey("InstructorId")]
        public Instructor? Instructor { get; set; }

        // Foreign key: Sınıf
        [Required(ErrorMessage = "Sınıf seçimi zorunludur.")]
        public int ClassroomId { get; set; }

        [ForeignKey("ClassroomId")]
        public Classroom? Classroom { get; set; }

        // Foreign key: Sınıf Düzeyi
        [Required(ErrorMessage = "Sınıf düzeyi seçimi zorunludur.")]
        public int GradeId { get; set; }

        [ForeignKey("GradeId")]
        public Grade? Grade { get; set; }

        // Foreign key: Dönem
        [Required(ErrorMessage = "Dönem seçimi zorunludur.")]
        public int TermId { get; set; }

        [ForeignKey("TermId")]
        public Term? Term { get; set; }

        
    }
}

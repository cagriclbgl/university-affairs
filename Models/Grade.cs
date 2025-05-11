namespace UniversityAffairs.Models
{
    public class Grade
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public ICollection<LessonSchedule>? LessonSchedules { get; set; }
    }
}

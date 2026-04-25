using System.ComponentModel.DataAnnotations;

namespace ContosoUniversity.WebApplication.Models.CourseModels
{
    public abstract class BaseCourseViewModel
    {
        public int CourseId { get; set; }
        public string? Title { get; set; }
        public int Credits { get; set; }
        public int DepartmentId { get; set; }
    }
}

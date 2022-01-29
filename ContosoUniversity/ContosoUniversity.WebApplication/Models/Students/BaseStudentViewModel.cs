using System.ComponentModel.DataAnnotations;

namespace ContosoUniversity.WebApplication.Models.Students
{
    public abstract class BaseStudentViewModel
    {
        public int Id { get; set; }
        [Required]
        public string? LastName { get; set; }
        [Required]
        public string? FirstName { get; set; }
        [Required]
        public DateTime EnrollmentDate { get; set; }
    }
}

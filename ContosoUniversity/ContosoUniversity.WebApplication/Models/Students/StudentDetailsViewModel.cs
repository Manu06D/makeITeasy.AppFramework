using ContosoUniversity.Models;
using makeITeasy.AppFramework.Core.Interfaces;

namespace ContosoUniversity.WebApplication.Models.Students
{
    public class StudentDetailsViewModel : BaseStudentViewModel, IMapFrom<Student>

    {
        public virtual IList<Enrollment>? Enrollments { get; set; }
    }
}

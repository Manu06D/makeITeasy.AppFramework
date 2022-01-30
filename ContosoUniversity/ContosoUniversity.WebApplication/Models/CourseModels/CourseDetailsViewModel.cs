using ContosoUniversity.Models;
using makeITeasy.AppFramework.Core.Interfaces;

namespace ContosoUniversity.WebApplication.Models.CourseModels
{
    public class CourseDetailsViewModel : BaseCourseViewModel, IMapFrom<Course>
    {
        public virtual Department Department { get; set; }
        public virtual IList<CourseAssignment> CourseAssignments { get; set; }
        public virtual IList<Enrollment> Enrollments { get; set; }
    }
}

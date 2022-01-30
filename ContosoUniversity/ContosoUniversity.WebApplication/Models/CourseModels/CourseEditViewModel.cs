using ContosoUniversity.Models;

using makeITeasy.AppFramework.Core.Interfaces;

namespace ContosoUniversity.WebApplication.Models.CourseModels
{
    public class CourseEditViewModel : BaseCourseViewModel, IMapFrom<Course>
    {
    }
}

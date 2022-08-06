using ContosoUniversity.Models;

using makeITeasy.AppFramework.Core.Interfaces;
using makeITeasy.AppFramework.Web.Attributes;

namespace ContosoUniversity.WebApplication.Models.Datatables
{
    public class StudentEnrollmentDatatableViewModel : IMapFrom<Enrollment>
    {

        [TableColumn(Name = nameof(EnrollmentId), Title = "EnrollmentId", IsRowId = true, Priority = 1, Visible = false)]
        public int EnrollmentId { get; set; }

        [TableColumn(Name = nameof(CourseTitle), Title = "Course Title",  SortDataSource = "Course.Title", Priority = 2)]
        public string CourseTitle { get; set; }

        [TableColumn(Name = nameof(Grade), Title = "Grade", Priority = 2)]
        public string? Grade { get; set; }
    }
}

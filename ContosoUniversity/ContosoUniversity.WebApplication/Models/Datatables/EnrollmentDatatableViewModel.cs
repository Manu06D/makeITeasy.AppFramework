using ContosoUniversity.Models;

using makeITeasy.AppFramework.Core.Interfaces;
using makeITeasy.AppFramework.Web.Attributes;

namespace ContosoUniversity.WebApplication.Models.Datatables
{
    public class EnrollmentDatatableViewModel : IMapFrom<Enrollment>
    {

        [TableColumn(Name = nameof(EnrollmentId), Title = "EnrollmentId", IsRowId = true, Priority = 1, Visible = false)]
        public int EnrollmentId { get; set; }

        [TableColumn(Name = nameof(CourseId), Title = "CourseId", Priority = 2)]
        public int CourseId { get; set; }

        [TableColumn(Name = nameof(StudentId), Title = "StudentId", Priority = 2)]
        public int StudentId { get; set; }

        [TableColumn(Name = nameof(Grade), Title = "Grade", Priority = 2)]
        public int Grade { get; set; }

        [TableColumn(Name = nameof(Edit), Title = "", Priority = 2, Sortable = false)]
        public string Edit { get; set; } = string.Empty;
    }
}

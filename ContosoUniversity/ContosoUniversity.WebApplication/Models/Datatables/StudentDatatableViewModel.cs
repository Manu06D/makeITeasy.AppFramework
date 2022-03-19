using ContosoUniversity.Models;

using makeITeasy.AppFramework.Core.Interfaces;
using makeITeasy.AppFramework.Web.Attributes;

namespace ContosoUniversity.WebApplication.Models.Datatables
{
    public class StudentDatatableViewModel : IMapFrom<Student>
    {
        [TableColumn(Name = nameof(ID), Title = "ID", IsRowId = true, Priority = 1, Visible = false)]
        public int ID { get; set; }

        [TableColumn(Name = nameof(LastName), Title = "LastName", Priority = 2)]
        public string LastName { get; set; }

        [TableColumn(Name = nameof(FirstName), Title = "FirstName", Priority = 2)]
        public string FirstName { get; set; }

        [TableColumn(Name = nameof(EnrollmentDate), Title = "EnrollmentDate", Priority = 2)]
        public string EnrollmentDate { get; set; }

        [TableColumn(Name = nameof(Edit), Title = "", Priority = 2, Sortable = false)]
        public string Edit { get; set; } = String.Empty;
    }
}

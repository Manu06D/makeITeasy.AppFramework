using AutoMapper;

using ContosoUniversity.Models;

using makeITeasy.AppFramework.Core.Interfaces;
using makeITeasy.AppFramework.Web.Attributes;

namespace ContosoUniversity.WebApplication.Models.Datatables
{
    public class InstructorDatatableViewModel : IMapFrom<Instructor>
    {

        [TableColumn(Name = nameof(Id), Title = "Id", IsRowId = true, Priority = 1, Visible = false)]
        public int Id { get; set; }

        [TableColumn(Name = nameof(LastName), Title = "LastName", Priority = 2)]
        public string LastName { get; set; }

        [TableColumn(Name = nameof(FirstName), Title = "FirstName", Priority = 2)]
        public string FirstName { get; set; }

        [TableColumn(Name = nameof(HireDate), Title = "HireDate", Priority = 2)]
        public DateTime HireDate { get; set; }

        [TableColumn(Name = nameof(OfficeAssignmentLocation), Title = "Office", SortDataSource = "OfficeAssignment.Location", Priority = 2)]
        public string OfficeAssignmentLocation { get; set; }

        [TableColumn(Name = nameof(Departments), Title = "Departments", Sortable = false, Priority = 2)]
        public ICollection<string> Departments { get; set; }

        //[TableColumn(Name = nameof(Edit), Title = "", Priority = 2, Sortable = false)]
        //public string Edit { get; set; } = string.Empty;

        public void Mapping(Profile profile)
        {
            if (profile != null)
            {
                profile.CreateMap<Instructor, InstructorDatatableViewModel>()
                    .ForMember(dest => dest.Departments, src => src.MapFrom(x => x.Departments.Select(x => x.Name)));
            }
        }
    }
}

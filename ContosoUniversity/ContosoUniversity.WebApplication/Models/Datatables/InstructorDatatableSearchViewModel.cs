using ContosoUniversity.Core.Queries.InstructorQueries;
using ContosoUniversity.WebApplication.Models.DepartmentModels;

namespace ContosoUniversity.WebApplication.Models.Datatables
{
    public class InstructorDatatableSearchViewModel : BasicInstructorQuery
    {
        public ICollection<ShortDepartementViewModel> Departements;
    }
}

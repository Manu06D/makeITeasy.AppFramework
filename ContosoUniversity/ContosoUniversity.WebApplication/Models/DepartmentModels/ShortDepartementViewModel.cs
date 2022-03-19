using ContosoUniversity.Models;

using makeITeasy.AppFramework.Core.Interfaces;

namespace ContosoUniversity.WebApplication.Models.DepartmentModels
{
    public class ShortDepartementViewModel : IMapFrom<Department>
    {
        public int DepartmentId { get; set; }
        public string Name { get; set; }
    }
}

using makeITeasy.AppFramework.Models;

namespace makeITeasy.CarCatalog.dotnet7.Models
{
    public partial class Brand : ITimeTrackingEntity
    {
        public object DatabaseID { get => Id; }
    }
}

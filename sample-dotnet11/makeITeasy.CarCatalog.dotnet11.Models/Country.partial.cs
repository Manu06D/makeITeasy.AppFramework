using makeITeasy.AppFramework.Models;

namespace makeITeasy.CarCatalog.dotnet11.Models
{
    public partial class Country : ITimeTrackingEntity
    {
        public object DatabaseID { get => Id; }
    }
}

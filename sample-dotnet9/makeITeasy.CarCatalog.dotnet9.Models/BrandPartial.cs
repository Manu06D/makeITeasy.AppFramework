using makeITeasy.AppFramework.Models;

namespace makeITeasy.CarCatalog.dotnet9.Models
{
    public partial class Brand : ITimeTrackingEntity
    {
        public object DatabaseID { get => Id; }
    }
}

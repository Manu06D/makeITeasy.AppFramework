using makeITeasy.AppFramework.Models;

namespace makeITeasy.CarCatalog.Models
{
    public partial class Country : ITimeTrackingEntity
    {
        public object DatabaseID { get => Id; }
    }
}

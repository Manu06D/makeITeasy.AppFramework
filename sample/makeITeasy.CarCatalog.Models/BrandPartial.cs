using makeITeasy.AppFramework.Models;

namespace makeITeasy.CarCatalog.Models
{
    public partial class Brand : ITimeTrackingEntity
    {
        public object DatabaseID { get => Id; }
    }
}

using makeITeasy.CarCatalog.dotnet6.Models.DynamicModels;

namespace makeITeasy.CarCatalog.dotnet6.Models
{
    public partial class CarDetail
    {
        public object DatabaseID { get => Id; }

        public CarDetailsModel CarDetails { get; set; }

    }
}

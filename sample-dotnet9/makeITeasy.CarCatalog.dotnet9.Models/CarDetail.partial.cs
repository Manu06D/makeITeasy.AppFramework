using makeITeasy.CarCatalog.dotnet9.Models.DynamicModels;

namespace makeITeasy.CarCatalog.dotnet9.Models
{
    public partial class CarDetail
    {
        public object DatabaseID { get => Id; }
        public CarDetailsModel CarDetails { get; set; }

    }
}

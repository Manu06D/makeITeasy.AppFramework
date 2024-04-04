using makeITeasy.CarCatalog.dotnet8.Models.DynamicModels;

namespace makeITeasy.CarCatalog.dotnet8.Models
{
    public partial class CarDetail
    {
        public object DatabaseID { get => Id; }
        public CarDetailsModel CarDetails { get; set; }

    }
}

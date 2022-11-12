using makeITeasy.CarCatalog.dotnet7.Models.DynamicModels;

namespace makeITeasy.CarCatalog.dotnet7.Models
{
    public partial class CarDetail
    {
        public object DatabaseID { get => Id; }

        public CarDetailsModel CarDetails { get; set; }

    }
}

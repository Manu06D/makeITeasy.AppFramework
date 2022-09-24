using makeITeasy.CarCatalog.Models.DynamicModels;

namespace makeITeasy.CarCatalog.Models
{
    public partial class CarDetail
    {
        public object DatabaseID { get => Id; }

        public CarDetailsModel CarDetails { get; set; }

    }
}

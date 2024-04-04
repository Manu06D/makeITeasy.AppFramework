using System.Reflection;

namespace makeITeasy.CarCatalog.Models
{
    public class CarCatalogModels
    {
        public static Assembly Assembly
        {
            get => typeof(CarCatalogModels).Assembly;
        }
    }
}

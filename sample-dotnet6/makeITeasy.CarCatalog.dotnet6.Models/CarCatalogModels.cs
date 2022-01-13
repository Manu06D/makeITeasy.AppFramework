
using System.Reflection;

namespace makeITeasy.CarCatalog.dotnet6.Models
{
    public class CarCatalogModels
    {
        public static Assembly Assembly
        {
            get => typeof(CarCatalogModels).Assembly;
        }
    }
}


using System.Reflection;

namespace makeITeasy.CarCatalog.dotnet6.Core
{
    public static class CarCatalogCore
    {
        public static Assembly Assembly
        {
            get => typeof(CarCatalogCore).Assembly;
        }
    }
}

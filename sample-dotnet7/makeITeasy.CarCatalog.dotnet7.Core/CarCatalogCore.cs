
using System.Reflection;
using makeITeasy.CarCatalog.dotnet7.Core;

namespace makeITeasy.CarCatalog.dotnet7.Core
{
    public static class CarCatalogCore
    {
        public static Assembly Assembly
        {
            get => typeof(CarCatalogCore).Assembly;
        }
    }
}

using System.Reflection;

namespace makeITeasy.CarCatalog.dotnet10.Core
{
    public static class CarCatalogCore
    {
        public static Assembly Assembly
        {
            get => typeof(CarCatalogCore).Assembly;
        }
    }
}

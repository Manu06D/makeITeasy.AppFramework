using System.Reflection;

namespace makeITeasy.CarCatalog.Core
{
    public static class CarCatalogCore
    {
        public static Assembly Assembly
        {
            get => typeof(CarCatalogCore).Assembly;
        }
    }
}

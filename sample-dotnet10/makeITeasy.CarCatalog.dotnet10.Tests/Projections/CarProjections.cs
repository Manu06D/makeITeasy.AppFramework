using Facet;
using Facet.Mapping;

using makeITeasy.CarCatalog.dotnet10.Models;

namespace makeITeasy.CarCatalog.dotnet10.Tests.Projections
{
    // Simple subset { Name }
    [Facet(typeof(Car), Include = [nameof(Car.Name)])]
    public partial class CarNameInfo { }

    // Subset { Id, Name }
    [Facet(typeof(Car), Include = [nameof(Car.Id), nameof(Car.Name)])]
    public partial class CarIdNameInfo { }

    // Flattening of nested navigations: BrandName <- Brand.Name, BrandCountryName <- Brand.Country.Name
    public class CarWithBrandInfoProjection : IFacetProjectionMapConfiguration<Car, CarWithBrandInfo>
    {
        public static void ConfigureProjection(IFacetProjectionBuilder<Car, CarWithBrandInfo> builder)
        {
            builder.Map(d => d.BrandName, s => s.Brand.Name);
            builder.Map(d => d.BrandCountryName, s => s.Brand.Country.Name);
        }
    }

    [Facet(typeof(Car), Include = [nameof(Car.Id), nameof(Car.Name)], Configuration = typeof(CarWithBrandInfoProjection))]
    public partial class CarWithBrandInfo
    {
        public string? BrandName { get; set; }
        public string? BrandCountryName { get; set; }
    }
}

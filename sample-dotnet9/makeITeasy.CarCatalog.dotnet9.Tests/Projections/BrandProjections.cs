using Facet;
using Facet.Mapping;

using makeITeasy.CarCatalog.dotnet9.Models;

namespace makeITeasy.CarCatalog.dotnet9.Tests.Projections
{
    // Subset { Id, Name }
    [Facet(typeof(Brand), Include = [nameof(Brand.Id), nameof(Brand.Name)])]
    public partial class BrandIdNameInfo { }

    // Computed aggregates: CarsCount <- Cars.Count, FirstRelease <- Cars.Min(ReleaseYear)
    public class BrandInfoProjection : IFacetProjectionMapConfiguration<Brand, BrandInfo>
    {
        public static void ConfigureProjection(IFacetProjectionBuilder<Brand, BrandInfo> builder)
        {
            builder.Map(d => d.CarsCount, s => s.Cars.Count);
            builder.Map(d => d.FirstRelease, s => s.Cars.Any() ? s.Cars.Min(c => c.ReleaseYear) : 0);
        }
    }

    [Facet(typeof(Brand), Include = [nameof(Brand.Name)], Configuration = typeof(BrandInfoProjection))]
    public partial class BrandInfo
    {
        public int CarsCount { get; set; }
        public int FirstRelease { get; set; }
    }

    // Nested facet collection: Cars <- Brand.Cars projected to CarNameInfo
    public class BrandWithCarsInfoProjection : IFacetProjectionMapConfiguration<Brand, BrandWithCarsInfo>
    {
        public static void ConfigureProjection(IFacetProjectionBuilder<Brand, BrandWithCarsInfo> builder)
        {
            builder.Map(d => d.Cars, s => s.Cars.Select(c => new CarNameInfo { Name = c.Name }).ToList());
        }
    }

    [Facet(typeof(Brand), Include = [nameof(Brand.Name)], Configuration = typeof(BrandWithCarsInfoProjection))]
    public partial class BrandWithCarsInfo
    {
        public List<CarNameInfo> Cars { get; set; } = [];
    }
}

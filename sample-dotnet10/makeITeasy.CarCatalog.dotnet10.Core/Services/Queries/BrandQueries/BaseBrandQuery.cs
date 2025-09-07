using makeITeasy.AppFramework.Models;
using makeITeasy.CarCatalog.dotnet10.Models;

namespace makeITeasy.CarCatalog.dotnet10.Core.Services.Queries.BrandQueries
{
    public class BasicBrandQuery : BaseQuery<Brand>
    {
        public int ID { get; set; }
        public string? NameSuffix { get; set; }

        public override void BuildQuery()
        {
            if (ID > 0)
            {
                AddFunctionToCriteria(x => x.Id == ID);
            }

            if (!string.IsNullOrWhiteSpace(NameSuffix))
            {
                AddFunctionToCriteria(x => x.Name.EndsWith(NameSuffix));
            }
        }
    }
}

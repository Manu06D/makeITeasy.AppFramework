using makeITeasy.AppFramework.Models;
using makeITeasy.CarCatalog.dotnet9.Models;

namespace makeITeasy.CarCatalog.dotnet9.Core.Services.Queries.CountryQueries
{
    public class BaseCountryQuery : BaseQuery<Country>
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

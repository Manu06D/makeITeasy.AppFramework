using makeITeasy.AppFramework.Models;
using makeITeasy.CarCatalog.dotnet8.Models;

namespace makeITeasy.CarCatalog.dotnet8.Core.Services.Queries.CountryQueries
{
    public class BaseCountryQuery : BaseQuery<Country>
    {
        public int ID { get; set; }

        public override void BuildQuery()
        {
            if (ID > 0)
            {
                AddFunctionToCriteria(x => x.Id == ID);
            }
        }
    }
}

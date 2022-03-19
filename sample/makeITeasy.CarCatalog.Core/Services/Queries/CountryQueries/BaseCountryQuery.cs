using makeITeasy.AppFramework.Models;
using makeITeasy.CarCatalog.Models;

namespace makeITeasy.CarCatalog.Core.Services.Queries.CountryQueries
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

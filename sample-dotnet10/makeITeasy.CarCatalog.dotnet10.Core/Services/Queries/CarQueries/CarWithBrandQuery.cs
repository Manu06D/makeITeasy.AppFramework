using makeITeasy.AppFramework.Models;
using makeITeasy.CarCatalog.dotnet10.Models;

namespace makeITeasy.CarCatalog.dotnet10.Core.Services.Queries.CarQueries
{
    public class CarWithBrandQuery : BaseTransactionQuery<Car>
    {
        public override void BuildQuery()
        {
            AddInclude(x => x.Brand);
        }
    }
}

using makeITeasy.AppFramework.Models;
using makeITeasy.CarCatalog.dotnet9.Models;

namespace makeITeasy.CarCatalog.dotnet9.Core.Services.Queries.CarQueries
{
    public class CarWithBrandQuery : BaseTransactionQuery<Car>
    {
        public override void BuildQuery()
        {
            AddInclude(x => x.Brand);
        }
    }
}

using makeITeasy.AppFramework.Models;
using makeITeasy.CarCatalog.dotnet9.Models;

namespace makeITeasy.CarCatalog.dotnet9.Core.Services.Queries.BrandQueries
{
    public class BasicBrandQuery : BaseQuery<Brand>
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

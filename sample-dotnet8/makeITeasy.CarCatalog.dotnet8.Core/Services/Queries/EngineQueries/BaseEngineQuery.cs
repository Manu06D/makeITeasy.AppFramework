using makeITeasy.AppFramework.Models;
using makeITeasy.CarCatalog.dotnet8.Models;

namespace makeITeasy.CarCatalog.dotnet8.Core.Services.Queries.EngineQueries
{
    public class BaseEngineQuery : BaseTransactionQuery<Engine>
    {
        public long? ID { get; set; }

        public string Name { get; set; }

        public override void BuildQuery()
        {
            if (ID.HasValue && ID.Value > 0)
            {
                AddFunctionToCriteria(x => x.Id == ID);
            }

            if (!string.IsNullOrWhiteSpace(Name))
            {
                AddFunctionToCriteria(x => x.Name.StartsWith(Name));
            }
        }
    } 
}

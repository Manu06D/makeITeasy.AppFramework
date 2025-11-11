using makeITeasy.AppFramework.Models;
using makeITeasy.CarCatalog.dotnet10.Models;

namespace makeITeasy.CarCatalog.dotnet10.Core.Services.Queries.EngineQueries
{
    public class BaseEngineQuery : BaseTransactionQuery<Engine>
    {
        public long? ID { get; set; }

        public string? Name { get; set; }
        public int? MinimalHorspower { get; set; }
        public Tuple<string,  string>? Characteristic { get; set; }
        public string? NameSuffix { get; set; }

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

            if (MinimalHorspower is not null)
            {
                AddFunctionToCriteria(x => x.Details.PowerHorse >= MinimalHorspower);
            }

            if (Characteristic is not null)
            {
                AddFunctionToCriteria(x => x.Details.Characteristics.Any(x => x.Name == Characteristic.Item1 && x.Value == Characteristic.Item2));
            }

            if (!string.IsNullOrWhiteSpace(NameSuffix))
            {
                AddFunctionToCriteria(x => x.Name.EndsWith(NameSuffix));
            }
        }
    }
}

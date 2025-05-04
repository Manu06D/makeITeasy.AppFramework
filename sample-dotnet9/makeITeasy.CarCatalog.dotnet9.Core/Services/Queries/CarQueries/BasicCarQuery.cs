using System.Linq.Expressions;

using makeITeasy.AppFramework.Core.Extensions;
using makeITeasy.AppFramework.Models;
using makeITeasy.CarCatalog.dotnet9.Models;

namespace makeITeasy.CarCatalog.dotnet9.Core.Services.Queries.CarQueries
{
    public class BasicCarQuery : BaseTransactionQuery<Car>
    {
        public long? ID { get; set; }

        public string? Name { get; set; }

        public bool? IsModernCar { get; set; }

        public bool? IncludeBrand { get; set; }

        public bool? IncludeBrandAndCountry { get; set; }

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

            if (!string.IsNullOrWhiteSpace(NameSuffix))
            {
                AddFunctionToCriteria(x => x.Name.EndsWith(NameSuffix));
            }

            if (IncludeBrandAndCountry.GetValueOrDefault())
            {
                AddInclude("Brand.Country");
            }
            else if (IncludeBrand.GetValueOrDefault())
            {
                AddInclude(x => x.Brand);
            }

            HandleNullableBoolSearch(IsModernCar, Car.ModernCarFunction);
        }

        public void HandleNullableBoolSearch(bool? searchValue, Expression<Func<Car, bool>> searchExp)
        {
            if (searchValue.HasValue)
            {
                if (searchValue.Value)
                {
                    AddFunctionToCriteria(searchExp);
                }
                else
                {
                    AddFunctionToCriteria(searchExp.Inverse());
                }
            }
        }
    }
}

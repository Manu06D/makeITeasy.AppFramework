using System;
using System.Linq.Expressions;

using makeITeasy.AppFramework.Core.Extensions;
using makeITeasy.AppFramework.Models;
using makeITeasy.CarCatalog.Models;

namespace makeITeasy.CarCatalog.Core.Services.Queries.CarQueries
{
    public class BaseCarQuery : BaseTransactionQuery<Car>
    {
        public long? ID { get; set; }

        public string Name { get; set; }

        public bool? IsModernCar { get; set; }

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

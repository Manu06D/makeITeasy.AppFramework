using System;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using makeITeasy.AppFramework.Core.Extensions;
using makeITeasy.AppFramework.Models;
using makeITeasy.CarCatalog.Models;

namespace makeITeasy.CarCatalog.Core.Services.Queries.CarQueries
{
    public class BaseCarQuery : BaseQuery<Car>
    {
        public long? ID { get; set; }

        public string Name { get; set; }

        public bool? IsModernCar { get; set; }

        public bool? IsItalianCar { get; set; }

        public override void BuildQuery()
        {
            if (ID.HasValue && ID.Value > 0)
            {
                AddFunctionToCriteria(x => x.Id == ID);
            }

            if(!string.IsNullOrWhiteSpace(Name))
            {
                AddFunctionToCriteria(x => x.Name.StartsWith(Name));
            }

            HandleNullableBoolSearch(IsModernCar, Car.ModernCarFunction);

            HandleNullableBoolSearch(IsItalianCar, Car.ItalianCarFunction);
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

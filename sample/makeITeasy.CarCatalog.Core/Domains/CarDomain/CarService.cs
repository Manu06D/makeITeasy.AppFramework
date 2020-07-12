using System;
using makeITeasy.AppFramework.Core.Services;
using makeITeasy.CarCatalog.Models;

namespace makeITeasy.CarCatalog.Core.Domains.CarDomain
{
    public class CarService : BaseEntityService<Car>, ICarService
    {
        public bool IsValid(Car car)
        {
            bool? result = this.ValidatorFactory.GetValidator<Car>()?.Validate(car).IsValid;

            return result.GetValueOrDefault(false);
        }
    }
}

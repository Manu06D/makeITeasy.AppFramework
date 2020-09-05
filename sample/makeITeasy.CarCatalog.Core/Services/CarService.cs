using makeITeasy.AppFramework.Core.Services;
using makeITeasy.CarCatalog.Models;
using makeITeasy.CarCatalog.Core.Services.Interfaces;


namespace makeITeasy.CarCatalog.Core.Services
{
    public class CarService : BaseEntityService<Car>, ICarService
    {
        public bool IsValid(Car car)
        {
            bool? result = ValidatorFactory.GetValidator<Car>()?.Validate(car).IsValid;

            return result.GetValueOrDefault(false);
        }
    }
}

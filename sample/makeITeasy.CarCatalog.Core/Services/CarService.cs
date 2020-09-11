using makeITeasy.AppFramework.Core.Services;
using makeITeasy.CarCatalog.Models;
using makeITeasy.CarCatalog.Core.Services.Interfaces;
using FluentValidation;
using makeITeasy.CarCatalog.Core.Ports;

namespace makeITeasy.CarCatalog.Core.Services
{
    public class CarService : BaseEntityService<Car>, ICarService
    {
        private readonly ICarRepository carRepository;

        public CarService(ICarRepository _carRepository, IValidatorFactory validatorFactory) : base(_carRepository, validatorFactory)
        {
            carRepository = _carRepository;
        }

        public void OwnServiceMethod()
        {
            string s = "hello world";
            carRepository.OwnRepositoryMethod();
        }
    }
}

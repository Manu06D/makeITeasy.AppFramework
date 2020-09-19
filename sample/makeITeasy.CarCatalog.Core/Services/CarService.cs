using makeITeasy.AppFramework.Core.Services;
using makeITeasy.CarCatalog.Models;
using makeITeasy.CarCatalog.Core.Services.Interfaces;
using FluentValidation;
using makeITeasy.CarCatalog.Core.Ports;
using System.Collections.Generic;
using makeITeasy.CarCatalog.Models.Custom;

namespace makeITeasy.CarCatalog.Core.Services
{
    public class CarService : BaseEntityService<Car>, ICarService
    {
        private readonly ICarRepository _carRepository;

        public CarService(ICarRepository carRepository, IValidatorFactory validatorFactory) : base(carRepository, validatorFactory)
        {
            _carRepository = carRepository;
        }

        public List<BrandGroupByCarCount> GetBrandWithCount()
        {
            return _carRepository.OwnRepositoryMethod();
        }
    }
}

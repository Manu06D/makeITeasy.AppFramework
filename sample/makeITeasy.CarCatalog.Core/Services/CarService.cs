using makeITeasy.AppFramework.Core.Services;
using makeITeasy.CarCatalog.Models;
using FluentValidation;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using makeITeasy.CarCatalog.Core.Ports;
using makeITeasy.CarCatalog.Core.Services.Interfaces;
using makeITeasy.CarCatalog.Models.Custom;

namespace makeITeasy.CarCatalog.Core.Services
{
    public class CarService : BaseEntityService<Car>, ICarService
    {
        private readonly ICarRepository _carRepository;

        public CarService(ICarRepository carRepository, ILogger<CarService> logger, IValidator<Car> validator = null) : base(carRepository, logger, validator)
        {
            _carRepository = carRepository;
        }

        public async Task<List<BrandGroupByCarCount>> GetBrandWithCountAsync()
        {
            return await _carRepository.GroupByBrandAndCountAsync();
        }
    }
}

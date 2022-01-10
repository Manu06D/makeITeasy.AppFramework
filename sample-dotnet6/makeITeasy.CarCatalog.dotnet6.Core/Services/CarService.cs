using makeITeasy.AppFramework.Core.Services;
using makeITeasy.CarCatalog.dotnet6.Models;
using FluentValidation;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using makeITeasy.CarCatalog.dotnet6.Core.Ports;
using makeITeasy.CarCatalog.dotnet6.Core.Services.Interfaces;
using makeITeasy.CarCatalog.dotnet6.Models.Custom;

namespace makeITeasy.CarCatalog.dotnet6.Core.Services
{
    public class CarService : BaseEntityService<Car>, ICarService
    {
        private readonly ICarRepository _carRepository;

        public CarService(ICarRepository carRepository, IValidatorFactory validatorFactory, ILogger<CarService> logger) : base(carRepository, validatorFactory, logger)
        {
            _carRepository = carRepository;
        }

        public async Task<List<BrandGroupByCarCount>> GetBrandWithCountAsync()
        {
            return await _carRepository.GroupByBrandAndCountAsync();
        }
    }
}

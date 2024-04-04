using makeITeasy.AppFramework.Core.Services;
using makeITeasy.CarCatalog.dotnet8.Models;
using FluentValidation;
using System.Collections.Generic;
using makeITeasy.CarCatalog.dotnet8.Models.Custom;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using makeITeasy.CarCatalog.dotnet8.Core.Ports;
using makeITeasy.CarCatalog.dotnet8.Core.Services.Interfaces;

namespace makeITeasy.CarCatalog.dotnet8.Core.Services
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

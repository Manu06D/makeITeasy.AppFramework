using makeITeasy.AppFramework.Core.Services;
using makeITeasy.CarCatalog.dotnet7.Models;
using FluentValidation;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using makeITeasy.CarCatalog.dotnet7.Models.Custom;
using makeITeasy.CarCatalog.dotnet7.Core.Ports;
using makeITeasy.CarCatalog.dotnet7.Core.Services.Interfaces;
using makeITeasy.CarCatalog.dotnet7.Core.Services;

namespace makeITeasy.CarCatalog.dotnet7.Core.Services
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

        public async Task XXX()
        {
            await _carRepository.UpdateRangeXXX();
        }
    }
}

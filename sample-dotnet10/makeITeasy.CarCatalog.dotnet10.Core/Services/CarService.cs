using FluentValidation;

using makeITeasy.AppFramework.Core.Services;
using makeITeasy.CarCatalog.dotnet10.Core.Ports;
using makeITeasy.CarCatalog.dotnet10.Core.Services.Interfaces;
using makeITeasy.CarCatalog.dotnet10.Models;
using makeITeasy.CarCatalog.dotnet10.Models.Custom;

using Microsoft.Extensions.Logging;

namespace makeITeasy.CarCatalog.dotnet10.Core.Services
{
    public class CarService : BaseEntityService<Car>, ICarService
    {
        private readonly ICarRepository _carRepository;
        private readonly ILogger<CarService> _logger;

        public CarService(ICarRepository carRepository, ILogger<CarService> logger, IValidator<Car> validator = null) : base(carRepository, logger, validator)
        {
            _carRepository = carRepository;
            _logger = logger;
        }

        public async Task<List<BrandGroupByCarCount>> GetBrandWithCountAsync()
        {
            using (_logger.BeginScope(new Dictionary<string, object>
            {
                ["Property1"] = 12345,
                ["Property1"] = "value2"
            }))
            {
                _logger.LogInformation("inside GetBrandWithCountAsync method");
                return await _carRepository.GroupByBrandAndCountAsync();
            }
        }
    }
}

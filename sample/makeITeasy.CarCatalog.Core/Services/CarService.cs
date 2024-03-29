﻿using makeITeasy.AppFramework.Core.Services;
using makeITeasy.CarCatalog.Models;
using makeITeasy.CarCatalog.Core.Services.Interfaces;
using FluentValidation;
using makeITeasy.CarCatalog.Core.Ports;
using System.Collections.Generic;
using makeITeasy.CarCatalog.Models.Custom;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

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

﻿using System;
using System.Linq;
using System.Threading.Tasks;

using FluentAssertions;

using makeITeasy.AppFramework.Core.Commands;
using makeITeasy.CarCatalog.dotnet9.Core.Services.Interfaces;
using makeITeasy.CarCatalog.dotnet9.Tests.Catalogs;
using makeITeasy.CarCatalog.dotnet9.Infrastructure.Data;
using makeITeasy.CarCatalog.dotnet9.Models;

using Xunit;
using System.Collections.Generic;
using Microsoft.Azure.Cosmos.Serialization.HybridRow;
using makeITeasy.CarCatalog.dotnet9.Core.Services.Queries.CarQueries;

namespace makeITeasy.CarCatalog.dotnet9.Tests
{
    public class ITimeTrackingEntity_Tests : UnitTestAutofacService<ServiceRegistrationAutofacModule>
    {
        private ICarService carService;
        private ICountryService countryService;

        public ITimeTrackingEntity_Tests()
        {
            var t = Resolve<CarCatalogContext>();

            t.Database.EnsureCreated();
            carService = Resolve<ICarService>();
            countryService = Resolve<ICountryService>();
        }

        ~ITimeTrackingEntity_Tests()
        {
            carService = null;
            countryService = null;
        }

        [Fact]
        public async Task CreationDate_BasicTest()
        {
            Car newCar = new Car()
            {
                Name = "C3",
                ReleaseYear = 2011,
                Brand = new Brand()
                {
                    Name = "Citroen",
                    Country = new Country()
                    {
                        Name = "France",
                        CountryCode = "FR"
                    }
                }
            };

            DateTime creationDateTime = DateTime.Now;

            CommandResult<Car> creationResult = await carService.CreateAsync(newCar);

            newCar.CreationDate.Should().NotBeNull().And.BeAfter(creationDateTime);

            creationResult.Entity.CreationDate.Should().NotBeNull().And.BeAfter(creationDateTime);
            creationResult.Entity.Brand.Country.CreationDate.Should().NotBeNull().And.BeAfter(creationDateTime);

            DateTime modificationDate = DateTime.Now;

            newCar.Name = "C4";

            CommandResult<Car> modificationResult = await carService.UpdateAsync(newCar);

            newCar.Name.Should().Be("C4");
            modificationResult.Entity.Name.Should().Be("C4");

            newCar.LastModificationDate.Should().NotBeNull().And.Be(modificationResult.Entity.LastModificationDate).And.BeAfter(modificationDate);

            newCar.Brand.Country.LastModificationDate.Should().NotBeNull().And.Be(modificationResult.Entity.Brand.Country.CreationDate);

            Car latestCar = await carService.GetByIdAsync(newCar.Id);

            latestCar.LastModificationDate.Should().NotBeNull().And.BeAfter(modificationDate);
        }

        [Fact]
        public async Task CreationRangeDate_BasicTest()
        {
            var carList = TestCarsCatalog.GetCars();

            var dbCreation = await carService.CreateRangeAsync(carList);

            dbCreation.Should().Match(x => x.All(y => y.Entity.CreationDate.HasValue));
        }

        [Fact]
        public async Task UpdateNofields_Test()
        {
            var carList = TestCarsCatalog.GetCars();

            DateTime dateTimeBeforeSave = DateTime.Now;
            var dbCreation = await carService.CreateAsync(carList.First());
            DateTime dateTimeAfterSave = DateTime.Now;

            DateTime? creationDateTime = dbCreation.Entity.CreationDate;
            DateTime? lastModificationDate = dbCreation.Entity.LastModificationDate;

            creationDateTime.Should().NotBeNull();
            creationDateTime.Value.Should().Be(lastModificationDate.Value).And.BeAfter(dateTimeBeforeSave).And.BeBefore(dateTimeAfterSave);

            var carQuery = await carService.GetFirstByQueryAsync(new BasicCarQuery() { ID = dbCreation.Entity.Id });

            await carService.UpdateAsync(carQuery);

            carQuery = await carService.GetFirstByQueryAsync(new BasicCarQuery() { ID = dbCreation.Entity.Id });

            carQuery.CreationDate.Should().Be(dbCreation.Entity.CreationDate);

            carQuery.LastModificationDate.Should().Be(dbCreation.Entity.LastModificationDate);
        }

        [Fact]
        public async Task UpdateWithfields_Test()
        {
            var carList = TestCarsCatalog.GetCars();

            DateTime dateTimeBeforeSave = DateTime.Now;
            var dbCreation = await carService.CreateAsync(carList.First());
            DateTime dateTimeAfterSave = DateTime.Now;

            DateTime? creationDateTime = dbCreation.Entity.CreationDate;
            DateTime? lastModificationDate = dbCreation.Entity.LastModificationDate;

            creationDateTime.Should().NotBeNull();
            creationDateTime.Value.Should().Be(lastModificationDate.Value).And.BeAfter(dateTimeBeforeSave).And.BeBefore(dateTimeAfterSave);

            var carQuery = await carService.GetFirstByQueryAsync(new BasicCarQuery() { ID = dbCreation.Entity.Id });

            carQuery.Name = "XXXx";

            DateTime dateBeforeUpdate = DateTime.Now;
            await carService.UpdateAsync(carQuery);
            DateTime dateAfterUpdate = DateTime.Now;

            carQuery.CreationDate.Should().Be(dbCreation.Entity.CreationDate);

            carQuery.LastModificationDate.Should().BeAfter(dateBeforeUpdate).And.BeBefore(dateAfterUpdate);
        }

        [Fact]
        public async Task UpdateProperties_LastModificationDateChanged()
        {
            Car newCar = new Car()
            {
                Name = "C3",
                ReleaseYear = 2011,
                Brand = new Brand()
                {
                    Name = "Citroen",
                    Country = new Country()
                    {
                        Name = "France",
                        CountryCode = "FR"
                    }
                }
            };

            DateTime creationDateTime = DateTime.Now;

            CommandResult<Car> creationResult = await carService.CreateAsync(newCar);

            newCar.Name += "X";

            await carService.UpdatePropertiesAsync(newCar, new string[] { "Name" });

            var tt = await carService.GetByIdAsync(newCar.Id);

            tt.LastModificationDate.GetValueOrDefault().Should().BeAfter(creationDateTime);
        }

        [Fact]
        public async Task CreationRangeDate_RecursiveTest()
        {
            DateTime dateTimeOfTest = DateTime.Now;

            Country country = new() { Name = "MyCountry", CountryCode = "MC", Brands = new List<Brand>() };

            country.Brands.Add(new Brand() { Name = "MyBrand", Cars = new List<Car>() { new Car() { Name = "MyCar" } } });

            var dbCreation = await countryService.CreateAsync(country);

            dbCreation.Entity.Brands.Should().Match(x => x.All(y => y.CreationDate.HasValue && y.CreationDate > dateTimeOfTest));
        }
    }
}

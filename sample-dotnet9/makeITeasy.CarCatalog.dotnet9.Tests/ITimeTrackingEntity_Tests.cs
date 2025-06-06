﻿using FluentAssertions;

using makeITeasy.AppFramework.Core.Commands;
using makeITeasy.CarCatalog.dotnet9.Core.Services.Interfaces;
using makeITeasy.CarCatalog.dotnet9.Models;

using Xunit;
using makeITeasy.CarCatalog.dotnet9.Core.Services.Queries.CarQueries;
using makeITeasy.CarCatalog.dotnet9.Tests.TestsSetup;
using makeITeasy.CarCatalog.dotnet9.Tests.Catalogs;
using makeITeasy.CarCatalog.dotnet9.Core.Services.Queries.BrandQueries;

namespace makeITeasy.CarCatalog.dotnet9.Tests
{
    public class ITimeTrackingEntity_Tests(DatabaseEngineFixture databaseEngineFixture) : UnitTestAutofacService(databaseEngineFixture)
    {
        [Fact]
        public async Task CreationDate_BasicTest()
        {
            DateTime creationDateTime = DateTime.Now;

            (ICarService carService, IBrandService brandService, Brand citroenBrand, string suffix, _) = await CreateCarsAsync();

            IList<Car> cars = (await carService.QueryAsync(new BasicCarQuery() { NameSuffix = suffix, IncludeBrandAndCountry = true })).Results;

            cars.Select(x => x.CreationDate).Should().AllSatisfy(x => x.Should().BeAfter(creationDateTime));
            cars.Select(x => x.Brand).Should().AllSatisfy(x => x.CreationDate.Should().BeAfter(creationDateTime)).And.AllSatisfy(x => x.Country.CreationDate.Should().BeAfter(creationDateTime));

            DateTime modificationDate = DateTime.Now;

            cars[0].Name += "Update";
            CommandResult<Car> modificationResult = await carService.UpdateAsync(cars.First());
            modificationResult.Result.Should().Be(CommandState.Success);
            modificationResult.Entity!.Name.Should().EndWith(suffix + "Update");

            Car modifiedCar = await carService.GetFirstByQueryAsync(new BasicCarQuery() { NameSuffix = suffix + "Update", IncludeBrandAndCountry = true });

            modifiedCar.LastModificationDate.Should().NotBeNull().And.Be(modificationResult.Entity.LastModificationDate).And.BeAfter(modificationDate);

            modifiedCar.Brand.Country.LastModificationDate.Should().NotBeNull().And.BeAfter(creationDateTime).And.BeBefore(modificationDate);
        }

        [Fact]
        public async Task CreationRangeDate_BasicTest()
        {
            ICarService carService = Resolve<ICarService>();
            string suffix = TimeOnly.FromDateTime(DateTime.Now).ToString("hhmmssfffffff");

            var dbCreation = await carService.CreateRangeAsync(new List<Car>() { CarsCatalog.CitroenC4(suffix), CarsCatalog.CitroenC5(suffix) });

            dbCreation.Should().Match(x => x.All(y => y.Entity.CreationDate.HasValue));
        }

        [Fact]
        public async Task EntityWithRowVersion_UpdateNofields_Test()
        {
            if (DatabaseEngineFixture.CurentDatabaseType == DatabaseType.MsSql)
            {
                ICarService carService = Resolve<ICarService>();
                string suffix = TimeOnly.FromDateTime(DateTime.Now).ToString("hhmmssfffffff");

                DateTime dateTimeBeforeSave = DateTime.Now;
                var dbCreation = await carService.CreateAsync(CarsCatalog.CitroenC4(suffix));
                DateTime dateTimeAfterSave = DateTime.Now;

                DateTime? creationDateTime = dbCreation.Entity.CreationDate;
                DateTime? lastModificationDate = dbCreation.Entity.LastModificationDate;

                creationDateTime.Should().NotBeNull();
                creationDateTime.Value.Should().Be(lastModificationDate.Value).And.BeAfter(dateTimeBeforeSave).And.BeBefore(dateTimeAfterSave);

                var carQuery = await carService.GetFirstByQueryAsync(new BasicCarQuery() { ID = dbCreation.Entity.Id });

                await carService.UpdateAsync(carQuery);

                carQuery = await carService.GetFirstByQueryAsync(new BasicCarQuery() { ID = dbCreation.Entity.Id });

                carQuery.CreationDate.Should().Be(dbCreation.Entity.CreationDate);

                carQuery.LastModificationDate.Value.Should().BeAfter(dbCreation.Entity.LastModificationDate.Value);
            }
        }

        [Fact]
        public async Task Entity_UpdateNofields_Test()
        {
            IBrandService brandService = Resolve<IBrandService>();
            string suffix = TimeOnly.FromDateTime(DateTime.Now).ToString("hhmmssfffffff");

            DateTime dateTimeBeforeSave = DateTime.Now;
            var dbCreation = await brandService.CreateAsync(CarsCatalog.Citroen(suffix));
            DateTime dateTimeAfterSave = DateTime.Now;

            DateTime? creationDateTime = dbCreation.Entity.CreationDate;
            DateTime? lastModificationDate = dbCreation.Entity.LastModificationDate;

            creationDateTime.Should().NotBeNull();
            creationDateTime.Value.Should().Be(lastModificationDate.Value).And.BeAfter(dateTimeBeforeSave).And.BeBefore(dateTimeAfterSave);

            var carQuery = await brandService.GetFirstByQueryAsync(new BasicBrandQuery() { ID = dbCreation.Entity.Id });

            await brandService.UpdateAsync(carQuery);

            carQuery = await brandService.GetFirstByQueryAsync(new BasicBrandQuery() { ID = dbCreation.Entity.Id });

            carQuery.CreationDate.Should().Be(dbCreation.Entity.CreationDate);

            carQuery.LastModificationDate.Should().Be(dbCreation.Entity.LastModificationDate);
        }

        [Fact]
        public async Task UpdateWithfields_Test()
        {
            ICarService carService = Resolve<ICarService>();
            string suffix = TimeOnly.FromDateTime(DateTime.Now).ToString("hhmmssfffffff");

            DateTime dateTimeBeforeSave = DateTime.Now;
            var dbCreation = await carService.CreateAsync(CarsCatalog.CitroenC4(suffix));
            DateTime dateTimeAfterSave = DateTime.Now;

            dbCreation.Entity.Should().NotBeNull();

            DateTime? creationDateTime = dbCreation.Entity.CreationDate;
            DateTime? lastModificationDate = dbCreation.Entity.LastModificationDate;

            creationDateTime.Should().NotBeNull();
            lastModificationDate.Should().NotBeNull();
            creationDateTime.Value.Should().Be(lastModificationDate.Value).And.BeAfter(dateTimeBeforeSave).And.BeBefore(dateTimeAfterSave);

            var carQuery = await carService.GetFirstByQueryAsync(new BasicCarQuery() { ID = dbCreation.Entity.Id });

            carQuery.Name += "XXXx";

            DateTime dateBeforeUpdate = DateTime.Now;
            await carService.UpdateAsync(carQuery);
            DateTime dateAfterUpdate = DateTime.Now;

            carQuery.CreationDate.Should().Be(dbCreation.Entity.CreationDate);

            carQuery.LastModificationDate.Should().BeAfter(dateBeforeUpdate).And.BeBefore(dateAfterUpdate);
        }

        [Fact]
        public async Task UpdateProperties_LastModificationDateChanged()
        {
            ICarService carService = Resolve<ICarService>();
            string suffix = TimeOnly.FromDateTime(DateTime.Now).ToString("hhmmssfffffff");

            Car newCar = CarsCatalog.CitroenC4(suffix);
            DateTime creationDateTime = DateTime.Now;

            CommandResult<Car> creationResult = await carService.CreateAsync(newCar);
            creationResult.Result.Should().Be(CommandState.Success);

            newCar.Name += "X";

            await carService.UpdatePropertiesAsync(newCar, ["Name"]);

            Car? carAfterUpdate = await carService.GetByIdAsync(newCar.Id);

            carAfterUpdate.Should().NotBeNull();
            carAfterUpdate.LastModificationDate.GetValueOrDefault().Should().BeAfter(creationDateTime);
        }

        [Fact]
        public async Task CreationRangeDate_RecursiveTest()
        {
            ICountryService countryService = Resolve<ICountryService>();
            string suffix = TimeOnly.FromDateTime(DateTime.Now).ToString("hhmmssfffffff");

            DateTime dateTimeOfTest = DateTime.Now;

            Country country = new() { Name = "MyCountry", CountryCode = "MC", Brands = [] };

            country.Brands.Add(new Brand() { Name = "MyBrand", Cars = [new Car() { Name = "MyCar" + suffix }] });

            var dbCreation = await countryService.CreateAsync(country);

            dbCreation.Entity.Should().NotBeNull();
            dbCreation.Entity.Brands.Should().Match(x => x.All(y => y.CreationDate.HasValue && y.CreationDate > dateTimeOfTest));
        }
    }
}

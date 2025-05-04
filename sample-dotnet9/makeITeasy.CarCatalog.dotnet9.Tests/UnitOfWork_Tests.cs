using FluentAssertions;

using makeITeasy.AppFramework.Core.Commands;
using makeITeasy.AppFramework.Core.Interfaces;
using makeITeasy.AppFramework.Core.Queries;
using makeITeasy.CarCatalog.dotnet9.Core.Services.Interfaces;
using makeITeasy.CarCatalog.dotnet9.Core.Services.Queries.BrandQueries;
using makeITeasy.CarCatalog.dotnet9.Core.Services.Queries.CarQueries;
using makeITeasy.CarCatalog.dotnet9.Models;
using makeITeasy.CarCatalog.dotnet9.Tests.TestsSetup;

using Microsoft.EntityFrameworkCore;

using Xunit;

namespace makeITeasy.CarCatalog.dotnet9.Tests
{
    public class UnitOfWork_Tests(DatabaseEngineFixture databaseEngineFixture) : UnitTestAutofacService(databaseEngineFixture)
    {
        [Fact]
        public async Task CreationUniqueName_ErrorTest()
        {
            ICarService carService = Resolve<ICarService>();
            IBrandService brandService = Resolve<IBrandService>();
            string suffix = TimeOnly.FromDateTime(DateTime.Now).ToString("hhmmssffff");

            Brand brand = new()
            {
                Name = "Citroen" + suffix,
                Country = new Country()
                {
                    Name = "France",
                    CountryCode = "FR"
                }
            };

            var brandCreationResult = await brandService.CreateAsync(brand);
            brandCreationResult.Result.Should().Be(CommandState.Success);
            brand.Id.Should().BePositive();

            Car car = new()
            {
                Name = "C4" + suffix,
                BrandId = brand.Id
            };

            var carCreationResult = await carService.CreateAsync(car);
            carCreationResult.Result.Should().Be(CommandState.Success);

            Car car2 = new()
            {
                Name = "C4" + suffix,
                BrandId = brand.Id
            };

            Func<Task> act = async () => await carService.CreateAsync(car2);

            await act.Should().ThrowAsync<DbUpdateException>();
        }

        [Fact]
        public async Task CreationUnitOfWork_WorkingTest()
        {
            ICarService carService = Resolve<ICarService>();
            IBrandService brandService = Resolve<IBrandService>();
            string suffix = TimeOnly.FromDateTime(DateTime.Now).ToString("hhmmssffff");

            Brand brand = new()
            {
                Name = "Citroen" + suffix,
                Country = new Country()
                {
                    Name = "France",
                    CountryCode = "FR"
                }
            };

            IUnitOfWork uo = Resolve<IUnitOfWork>();

            var brandRepository = uo.GetRepository<Brand>();
            var carRepository = uo.GetRepository<Car>();

            _ = await brandRepository.AddAsync(brand, false);

            Car car = new()
            {
                Name = "C4" + suffix,
                Brand = brand
            };

            _ = await carRepository.AddAsync(car, false);

            car.Id.Should().Be(0);
            brand.Id.Should().Be(0);

            int saveResult = await uo.CommitAsync();

            saveResult.Should().Be(3);

            car.Id.Should().BePositive();
            brand.Id.Should().BePositive();

            var brandSearch = await brandService.QueryAsync(new BasicBrandQuery() { NameSuffix = suffix});
            brandSearch.Results.Should().HaveCount(1);

            var query = new BasicCarQuery() { NameSuffix = suffix};
            query.AddInclude(x => x.Brand);

            var carSearch = await carService.QueryAsync(query);
            carSearch.Results.Should().HaveCount(1);
            carSearch.Results[0].Name.Should().Be("C4" + suffix);
            carSearch.Results[0].Brand.Name.Should().Be("Citroen" + suffix);
        }

        [Fact]
        public async Task CreationUnitOfWork_ErrorTest()
        {
            IBrandService brandService = Resolve<IBrandService>();
            string suffix = TimeOnly.FromDateTime(DateTime.Now).ToString("hhmmssffff");

            Brand brand = new()
            {
                Name = "Citroen" + suffix,
                Country = new Country()
                {
                    Name = "France",
                    CountryCode = "FR"
                }
            };

            IUnitOfWork uo = Resolve<IUnitOfWork>();

            var brandRepository = uo.GetRepository<Brand>();
            var carRepository = uo.GetRepository<Car>();

            await brandRepository.AddAsync(brand, false);

            Car car = new()
            {
                Name = "C4" + suffix,
                Brand = brand
            };

            Car car2 = new()
            {
                Name = "C4" + suffix,
                Brand = brand
            };

            await carRepository.AddAsync(car, false);
            await carRepository.AddAsync(car2, false);

            int dbResult = await uo.CommitAsync();

            dbResult.Should().BeLessThanOrEqualTo(0);
            QueryResult<Brand> brandSearch = await brandService.QueryAsync(new BasicBrandQuery() { NameSuffix = suffix});
            brandSearch.Results.Should().HaveCount(0);
        }
    }
}

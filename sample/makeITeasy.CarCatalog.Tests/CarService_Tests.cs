using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using makeITeasy.AppFramework.Core.Interfaces;
using makeITeasy.AppFramework.Core.Queries;
using makeITeasy.CarCatalog.Core.Domains.CarDomain;
using makeITeasy.CarCatalog.Core.Domains.CarDomain.Queries;
using makeITeasy.CarCatalog.Infrastructure.Data;
using makeITeasy.CarCatalog.Models;
using makeITeasy.CarCatalog.Tests.Catalogs;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace makeITeasy.CarCatalog.Tests
{
    public class CarService_Tests : UnitTestAutofacService<ServiceRegistrationAutofacModule>
    {
        private ICarService carService;

        public CarService_Tests()
        {
            carService = Resolve<ICarService>();

            var t = Resolve<CarCatalogContext>();
            t.Database.EnsureCreated();
        }

        ~CarService_Tests()
        {
            carService = null;
        }

        [Fact]
        public void IsValid_InValidObjectTest()
        {
            var newCar = new Car
            {
                Name = "x"
            };

            carService.IsValid(newCar).Should().BeFalse();
        }

        [Fact]
        public void IsValid_ValidObjectTest()
        {
            var newCar = new Car
            {
                Name = "xxx"
            };

            carService.IsValid(newCar).Should().BeTrue();
        }

        [Fact]
        public async Task Create_InValidObjectTest()
        {
            var newCar = new Car
            {
                Name = "x"
            };

            var result = await carService.Create(newCar);

            result.Result.Should().Be(CommandState.Error);
        }

        [Fact]
        public void Create_UncompleteObjectTest()
        {
            var newCar = new Car
            {
                Name = "xxx"
            };

            carService.Invoking(y => y.Create(newCar)).Should().Throw<DbUpdateException>();
        }


        [Fact]
        public async Task CreateAndGet_BasicTest()
        {
            var newCar = TestCarsCatalog.GetValidCar();

            var result = await carService.Create(newCar);

            result.Result.Should().Be(CommandState.Success);

            var getResult = await carService.QueryAsync(new BaseCarQuery() { ID = result.Entity.Id}, includeCount : true);

            getResult.TotalItems.Should().Be(1);
            getResult.Results.Should().NotBeEmpty().And.HaveCount(1);
            getResult.Results.Should().SatisfyRespectively(
                first =>
                {
                    first.Id.Should().BeGreaterThan(0);
                    first.Name.Should().Be(newCar.Name);
                });
        }

        [Fact]
        public async Task CreateAndGet_ListTest()
        {
            var newCars = TestCarsCatalog.GetValidCars(50);

            newCars.ForEach(async x => await carService.Create(x));

            var getResult = await carService.QueryAsync(new BaseCarQuery(), includeCount: true);

            getResult.TotalItems.Should().Be(newCars.Count);

            getResult.Results.Select(x => x.Id).Should().BeInAscendingOrder();
        }

        [Fact]
        public async Task CreateAndGet_ListWithIncludeStringTest()
        {
            var newCars = TestCarsCatalog.GetValidCars(50);

            newCars.ForEach(async x => await carService.Create(x));

            var getResult = await carService.QueryAsync(new BaseCarQuery() { IncludeStrings = new List<string>() { "Brand.Country"} }, includeCount: true);

            getResult.TotalItems.Should().Be(newCars.Count);

            getResult.Results.Select(x => x.Id).Should().BeInAscendingOrder();

            getResult.Results.Should().OnlyContain(x => x.Brand.Country != null);
        }

        [Fact]
        public async Task CreateAndGet_ListWithIncludeTest()
        {
            var newCars = TestCarsCatalog.GetValidCars(50);

            newCars.ForEach(async x => await carService.Create(x));

            var getResult = await carService.QueryAsync
                (new BaseCarQuery() { Includes=new List<System.Linq.Expressions.Expression<Func<Car, object>>>(){ x => x.Brand.Country } }, includeCount: true);

            getResult.TotalItems.Should().Be(newCars.Count);

            getResult.Results.Select(x => x.Id).Should().BeInAscendingOrder();

            getResult.Results.Should().OnlyContain(x => x.Brand.Country != null);
        }

        public class SmallCarInfo : IMapFrom<Car>
        {
            public long ID { get; set; }
            public string Name { get; set; }
        }

        [Fact]
        public async Task CreateAndGet_ListWithMappingTest()
        {
            var newCars = TestCarsCatalog.GetValidCars(50);

            newCars.ForEach(async x => await carService.Create(x));

            var getResult = await carService.QueryWithProjectionAsync<SmallCarInfo>
                (new BaseCarQuery() { }, includeCount: true);

            getResult.TotalItems.Should().Be(newCars.Count);

            getResult.Results.Select(x => x.ID).Should().BeInAscendingOrder();

            getResult.Results.Should().OnlyContain(x => x.Name != null);

        }

        public class SmallCarInfoWithBrand : IMapFrom<Car>
        {
            public long ID { get; set; }
            public string Name { get; set; }
            public string BrandName { get; set; }
        }

        [Fact]
        public async Task CreateAndGet_ListWithFunctionTest()
        {
            var newCars = TestCarsCatalog.GetValidCars(50);

            newCars.ForEach(async x => await carService.Create(x));

            var getResult = await carService.QueryAsync
                (new BaseCarQuery() {IsModernCar = true }, includeCount: true);

            getResult.TotalItems.Should().Be(newCars.Count(x => x.ReleaseYear > 2000));
        }


        [Fact]
        public async Task CreateAndGet_ListWithMapping2LevelTest()
        {
            var newCars = TestCarsCatalog.GetValidCars(50);

            newCars.ForEach(async x => await carService.Create(x));

            var getResult = await carService.QueryWithProjectionAsync<SmallCarInfoWithBrand>
                (new BaseCarQuery() { }, includeCount: true);

            getResult.TotalItems.Should().Be(newCars.Count);

            getResult.Results.Select(x => x.ID).Should().BeInAscendingOrder();

            getResult.Results.Should().OnlyContain(x => x.Name != null);
            getResult.Results.Should().OnlyContain(x => x.BrandName != null);
        }
    }
}

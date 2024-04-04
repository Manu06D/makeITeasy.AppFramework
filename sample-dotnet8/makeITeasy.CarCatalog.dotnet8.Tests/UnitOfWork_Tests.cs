using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FluentAssertions;

using makeITeasy.AppFramework.Core.Commands;
using makeITeasy.AppFramework.Core.Interfaces;
using makeITeasy.CarCatalog.dotnet8.Core.Services.Interfaces;
using makeITeasy.CarCatalog.dotnet8.Core.Services.Queries.BrandQueries;
using makeITeasy.CarCatalog.dotnet8.Core.Services.Queries.CarQueries;
using makeITeasy.CarCatalog.dotnet8.Infrastructure.Data;
using makeITeasy.CarCatalog.dotnet8.Models;

using Microsoft.EntityFrameworkCore;

using Xunit;

namespace makeITeasy.CarCatalog.dotnet8.Tests
{
    public class UnitOfWork_Tests : UnitTestAutofacService<ServiceRegistrationAutofacModule>
    {
        private ICarService carService;
        private IBrandService brandService;

        public UnitOfWork_Tests()
        {
            var t = Resolve<CarCatalogContext>();

            t.Database.EnsureCreated();
            carService = Resolve<ICarService>();
            brandService = Resolve<IBrandService>();
        }

        ~UnitOfWork_Tests()
        {
            carService = null;
        }

        [Fact]
        public async Task CreationUniqueName_ErrorTest()
        {
            Brand brand = new Brand()
            {
                Name = "Citroen",
                Country = new Country()
                {
                    Name = "France",
                    CountryCode = "FR"
                }
            };

            var brandCreationResult = await brandService.CreateAsync(brand);
            brandCreationResult.Result.Should().Be(CommandState.Success);
            brand.Id.Should().BePositive();

            Car car = new Car()
            {
                Name = "C4",
                BrandId = brand.Id
            };

            var carCreationResult = await carService.CreateAsync(car);
            carCreationResult.Result.Should().Be(CommandState.Success);

            Car car2 = new Car()
            {
                Name = "C4",
                BrandId = brand.Id
            };

            Func<Task> act = async () => await carService.CreateAsync(car2);

            act.Should().ThrowAsync<DbUpdateException>();
        }

        [Fact]
        public async Task CreationUnitOfWork_WorkingTest()
        {
            Brand brand = new Brand()
            {
                Name = "Citroen",
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

            Car car = new Car()
            {
                Name = "C4",
                Brand = brand
            };

            _ = await carRepository.AddAsync(car, false);

            car.Id.Should().Be(0);
            brand.Id.Should().Be(0);

            int saveResult = await uo.CommitAsync();

            saveResult.Should().Be(3);

            car.Id.Should().BePositive();
            brand.Id.Should().BePositive();

            var brandSearch = await brandService.QueryAsync(new BaseBrandQuery());
            brandSearch.Results.Should().HaveCount(1);

            var query = new BaseCarQuery();
            query.AddInclude(x => x.Brand);

            var carSearch = await carService.QueryAsync(query);
            carSearch.Results.Should().HaveCount(1);
            carSearch.Results.First().Name.Should().Be("C4");
            carSearch.Results.First().Brand.Name.Should().Be("Citroen");
        }

        [Fact]
        public async Task CreationUnitOfWork_ErrorTest()
        {
            Brand brand = new Brand()
            {
                Name = "Citroen",
                Country = new Country()
                {
                    Name = "France",
                    CountryCode = "FR"
                }
            };

            IUnitOfWork uo = Resolve<IUnitOfWork>();

            var brandRepository = uo.GetRepository<Brand>();
            var carRepository = uo.GetRepository<Car>();

            var a = await brandRepository.AddAsync(brand, false);

            Car car = new Car()
            {
                Name = "C4",
                Brand = brand
            };

            Car car2 = new Car()
            {
                Name = "C4",
                Brand = brand
            };

            var b = await carRepository.AddAsync(car, false);
            var c = await carRepository.AddAsync(car2, false);

            int i = await uo.CommitAsync();

            var brandSearch = await brandService.QueryAsync(new BaseBrandQuery());
        }
    }
}

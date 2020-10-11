using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using makeITeasy.AppFramework.Core.Commands;
using makeITeasy.CarCatalog.Core.Services.Interfaces;
using makeITeasy.CarCatalog.Infrastructure.Data;
using makeITeasy.CarCatalog.Models;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace makeITeasy.CarCatalog.Tests
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
        public async Task CreationDate_BasicTest()
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

            act.Should().Throw<DbUpdateException>();
        }
    }
}

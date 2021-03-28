using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using makeITeasy.AppFramework.Core.Commands;
using makeITeasy.CarCatalog.Core.Services.Interfaces;
using makeITeasy.CarCatalog.Infrastructure.Data;
using makeITeasy.CarCatalog.Models;
using makeITeasy.CarCatalog.Tests.Catalogs;
using Xunit;

namespace makeITeasy.CarCatalog.Tests
{
    public class ITimeTrackingEntity_Tests : UnitTestAutofacService<ServiceRegistrationAutofacModule>
    {
        private ICarService carService;

        public ITimeTrackingEntity_Tests()
        {
            var t = Resolve<CarCatalogContext>();

            t.Database.EnsureCreated();
            carService = Resolve<ICarService>();
        }

        ~ITimeTrackingEntity_Tests()
        {
            carService = null;
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
    }
}

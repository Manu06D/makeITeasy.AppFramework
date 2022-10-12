using FluentAssertions;

using makeITeasy.AppFramework.Core.Commands;
using makeITeasy.CarCatalog.Core.Services.Interfaces;
using makeITeasy.CarCatalog.Core.Services.Queries.CarQueries;
using makeITeasy.CarCatalog.Infrastructure.Data;
using makeITeasy.CarCatalog.Models;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xunit;

namespace makeITeasy.CarCatalog.Tests
{
    public class ServiceUpdate_Tests : UnitTestAutofacService<ServiceRegistrationAutofacModule>
    {
        private ICarService carService;

        public ServiceUpdate_Tests()
        {
            carService = Resolve<ICarService>();

            var t = Resolve<CarCatalogContext>();
            t.Database.EnsureCreated();
        }

        ~ServiceUpdate_Tests()
        {
            carService = null;
        }

        [Fact]
        public async Task UpdateSameObject_Test()
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

            var result = await carService.CreateAsync(newCar);

            var queryResult = await carService.GetFirstByQueryAsync(new BaseCarQuery() { ID = result.Entity.Id });

            DateTime? firstLastModificationDate = queryResult.LastModificationDate;

            await Task.Delay(10);
            var resultUpdate = await carService.UpdateAsync(newCar);
            resultUpdate.Result.Should().Be(CommandState.Success);

            queryResult = await carService.GetFirstByQueryAsync(new BaseCarQuery() { ID = result.Entity.Id });

            DateTime? secondLastModificationDate = queryResult.LastModificationDate;

            firstLastModificationDate.Should().Be(secondLastModificationDate);
        }


        [Fact]
        public async Task UpdateSameProperties_Test()
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

            var result = await carService.CreateAsync(newCar);

            var queryResult = await carService.GetFirstByQueryAsync(new BaseCarQuery() { ID = result.Entity.Id });

            DateTime? firstLastModificationDate = queryResult.LastModificationDate;

            await Task.Delay(10);
            var resultUpdate = await carService.UpdatePropertiesAsync(newCar, new string[] {nameof(Car.Name)});
            resultUpdate.Result.Should().Be(CommandState.Success);

            queryResult = await carService.GetFirstByQueryAsync(new BaseCarQuery() { ID = result.Entity.Id });

            DateTime? secondLastModificationDate = queryResult.LastModificationDate;

            firstLastModificationDate.Should().Be(secondLastModificationDate);
        }
    }
}

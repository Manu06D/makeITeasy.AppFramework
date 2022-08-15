using FluentAssertions;

using makeITeasy.AppFramework.Core.Commands;
using makeITeasy.CarCatalog.dotnet6.Core.Services.Interfaces;
using makeITeasy.CarCatalog.dotnet6.Core.Services.Queries.CarQueries;
using makeITeasy.CarCatalog.dotnet6.Infrastructure.Data;
using makeITeasy.CarCatalog.dotnet6.Models;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xunit;

namespace makeITeasy.CarCatalog.dotnet6.Tests
{
    public class RowVersion_Tests : UnitTestAutofacService<ServiceRegistrationAutofacModule>
    {
        private ICarService carService;

        public RowVersion_Tests()
        {
            carService = Resolve<ICarService>();
            var t = Resolve<CarCatalogContext>();

            t.Database.EnsureCreated();

            t.Database.ExecuteSqlRaw(@"CREATE TRIGGER CreateCarVersion  AFTER INSERT ON Car  BEGIN  UPDATE Car SET Version = 1 WHERE rowid = NEW.rowid;  END");
            t.Database.ExecuteSqlRaw("CREATE TRIGGER UpdateCarVersion AFTER UPDATE ON Car BEGIN UPDATE Car SET Version = Version + 1 WHERE rowid = NEW.rowid; END;");
        }

        [Fact]
        public async Task CreateAndGet_BasicRowVersionTest()
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

            result.Result.Should().Be(CommandState.Success);

            await Task.Delay(25);

            var afterFirstUpdateQueryResult = await carService.QueryAsync(new BaseCarQuery() { ID = result.Entity.Id }, includeCount: true);

            afterFirstUpdateQueryResult.Results.First().Version.Should().BeGreaterThan(0);

            afterFirstUpdateQueryResult.Results.First().Name = "C4";
            result = await carService.UpdateAsync(afterFirstUpdateQueryResult.Results.First());

            await Task.Delay(25);

            var afterSecondUpdateQueryResult = await carService.QueryAsync(new BaseCarQuery() { ID = result.Entity.Id }, includeCount: true);

            afterFirstUpdateQueryResult.Results.First().Version.Should().NotBe(afterSecondUpdateQueryResult.Results.First().Version);
        }

        [Fact]
        public async Task SameObjectUpdate_RowVersionTest()
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

            result.Result.Should().Be(CommandState.Success);

            await Task.Delay(25);

            var afterFirstUpdateQueryResult = await carService.QueryAsync(new BaseCarQuery() { ID = result.Entity.Id }, includeCount: true);

            afterFirstUpdateQueryResult.Results.First().Version.Should().BeGreaterThan(0);

            result = await carService.UpdateAsync(afterFirstUpdateQueryResult.Results.First());
            
            result.Result.Should().Be(CommandState.Warning);
            
            await Task.Delay(25);

            var afterSecondUpdateQueryResult = await carService.QueryAsync(new BaseCarQuery() { ID = result.Entity.Id }, includeCount: true);

            afterFirstUpdateQueryResult.Results.First().Version.Should().Be(afterSecondUpdateQueryResult.Results.First().Version);

            afterSecondUpdateQueryResult.Results.First().Name = "C4";

            result = await carService.UpdateAsync(afterSecondUpdateQueryResult.Results.First());

            var afterThirdUpdateQueryResult = await carService.QueryAsync(new BaseCarQuery() { ID = result.Entity.Id }, includeCount: true);

            afterThirdUpdateQueryResult.Results.First().Version.Should().BeGreaterThan(afterSecondUpdateQueryResult.Results.First().Version);

        }
    }
}

using AwesomeAssertions;

using makeITeasy.CarCatalog.dotnet10.Core.Ports;
using makeITeasy.CarCatalog.dotnet10.Core.Services.Interfaces;
using makeITeasy.CarCatalog.dotnet10.Core.Services.Queries.CarQueries;
using makeITeasy.CarCatalog.dotnet10.Models;
using makeITeasy.CarCatalog.dotnet10.Tests.Catalogs;
using makeITeasy.CarCatalog.dotnet10.Tests.TestsSetup;

using Microsoft.EntityFrameworkCore;

using Xunit;

namespace makeITeasy.CarCatalog.dotnet10.Tests
{
    public class TransactionStrategy_Tests(DatabaseFixture databaseEngineFixture) : AutofacFixture(databaseEngineFixture)
    {
        [Fact]
        public async Task AddCarsInResilientTransaction_CommitsAllChanges()
        {
            ICarRepository carRepository = Resolve<ICarRepository>();
            ICarService carService = Resolve<ICarService>();
            string suffix = TimeOnly.FromDateTime(DateTime.Now).ToString("hhmmssfffffff");

            Brand brand = CarsCatalog.Citroen(suffix, country: CarsCatalog.France);
            List<Car> cars =
            [
                CarsCatalog.CitroenC4(suffix, brand: brand),
                CarsCatalog.CitroenC5(suffix, brand: brand)
            ];

            int savedRows = await carRepository.AddCarsInResilientTransactionAsync(cars, TestContext.Current.CancellationToken);

            savedRows.Should().Be(4); //cars + brands

            var searchResult = await carService.QueryAsync(new BasicCarQuery() { NameSuffix = suffix });
            searchResult.Results.Should().HaveCount(2);
            searchResult.Results.Select(c => c.Name).Should().Contain(["C4" + suffix, "C5" + suffix]);
        }

        [Fact]
        public async Task AddCarsInResilientTransaction_OnFailure_RollsBackEverything()
        {
            ICarRepository carRepository = Resolve<ICarRepository>();
            ICarService carService = Resolve<ICarService>();
            string suffix = TimeOnly.FromDateTime(DateTime.Now).ToString("hhmmssfffffff");

            Brand brand = CarsCatalog.Citroen(suffix, country: CarsCatalog.France);

            List<Car> cars =
            [
                CarsCatalog.CitroenC4(suffix, brand: brand),
                CarsCatalog.CitroenC4(suffix, brand: brand) // should raise exception because of duplicate name
            ];

            Func<Task> act = async () => await carRepository.AddCarsInResilientTransactionAsync(cars, TestContext.Current.CancellationToken);

            await act.Should().ThrowAsync<DbUpdateException>();

            var searchResult = await carService.QueryAsync(new BasicCarQuery() { NameSuffix = suffix });
            searchResult.Results.Should().BeEmpty();
        }
    }
}

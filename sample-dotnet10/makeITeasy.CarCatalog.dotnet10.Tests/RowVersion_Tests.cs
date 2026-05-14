using AwesomeAssertions;

using makeITeasy.AppFramework.Core.Commands;
using makeITeasy.CarCatalog.dotnet10.Core.Services.Interfaces;
using makeITeasy.CarCatalog.dotnet10.Core.Services.Queries.CarQueries;
using makeITeasy.CarCatalog.dotnet10.Models;
using makeITeasy.CarCatalog.dotnet10.Tests.Catalogs;
using makeITeasy.CarCatalog.dotnet10.Tests.TestsSetup;

using Microsoft.EntityFrameworkCore;

using System.Text.Json;

using Xunit;

namespace makeITeasy.CarCatalog.dotnet10.Tests
{
    public class RowVersion_Tests(DatabaseFixture databaseEngineFixture) : AutofacFixture(databaseEngineFixture)
    {
        [Fact]
        public async Task CreateAndGet_BasicRowVersionTest()
        {
            if (GlobalTestSetup.DatabaseType == DatabaseType.MsSql)
            {
                ICarService carService = Resolve<ICarService>();
                string suffix = TimeOnly.FromDateTime(DateTime.Now).ToString("hhmmssfffffff");

                var result = await carService.CreateAsync(CarsCatalog.CitroenC4(suffix));

                result.Result.Should().Be(CommandState.Success);

                await Task.Delay(25, TestContext.Current.CancellationToken);

                Car carAfterFirstUpdate = await carService.GetFirstByQueryAsync(new BasicCarQuery() { ID = result.Entity!.Id });

                //afterFirstUpdateQueryResult.Results.First().Version.Should().BeGreaterThan(0);

                string rowVersionOfCarAfterFirstUpdate = BitConverter.ToString(carAfterFirstUpdate.RowVersion);

                carAfterFirstUpdate.Name += "XX";
                result = await carService.UpdateAsync(carAfterFirstUpdate);

                await Task.Delay(25, TestContext.Current.CancellationToken);

                Car afterSecondUpdateQueryResult = await carService.GetFirstByQueryAsync(new BasicCarQuery() { ID = result.Entity.Id });
                string rowVersionOfCarAfterSecondUpdate = BitConverter.ToString(afterSecondUpdateQueryResult.RowVersion);

                rowVersionOfCarAfterSecondUpdate.Should().NotBe(rowVersionOfCarAfterFirstUpdate);
            }
        }

        [Fact]
        public async Task RowVersion_Test()
        {
            (ICarService carService, _, _, string suffix, _) = await CarsCatalog.CreateCarsAsync(this);

            //This will not work on sql server but work here on sql lite cause lack of support of rowversion
            if (GlobalTestSetup.DatabaseType == DatabaseType.MsSql)
            {
                Car firstCar = await carService.GetFirstByQueryAsync(new BasicCarQuery() { NameSuffix = suffix });
                Car secondCar = JsonSerializer.Deserialize<Car>(JsonSerializer.Serialize(firstCar));

                firstCar.Should().NotBeNull();
                secondCar.Should().NotBeNull();

                string firstCarRowVersion = BitConverter.ToString(firstCar.RowVersion);
                firstCarRowVersion.Should().Be(BitConverter.ToString(secondCar.RowVersion));
                firstCar.RowVersion.Should().BeEquivalentTo(secondCar.RowVersion);

                firstCar!.Name += "Test";
                var t = await carService.UpdateAsync(firstCar);

                //firstcar should been updated with new row
                //firstCarRowVersion.Should().NotBeEquivalentTo(BitConverter.ToString(firstCar.RowVersion));

                Car firstCarAterUpdate = await carService.GetFirstByQueryAsync(new BasicCarQuery() { NameSuffix = suffix + "Test" });
                firstCarAterUpdate.Name.Should().EndWith("Test");
                firstCarRowVersion.Should().NotBeEquivalentTo(BitConverter.ToString(firstCarAterUpdate.RowVersion));

                secondCar!.Name += " 2";

                await carService.Invoking(y => y.UpdateAsync(secondCar)).Should().ThrowAsync<DbUpdateConcurrencyException>();
            }
        }
    }
}

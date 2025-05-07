//using FluentAssertions;

using FluentAssertions;

using makeITeasy.AppFramework.Core.Commands;
using makeITeasy.CarCatalog.dotnet9.Core.Services.Interfaces;
using makeITeasy.CarCatalog.dotnet9.Core.Services.Queries.CarQueries;
using makeITeasy.CarCatalog.dotnet9.Models;
using makeITeasy.CarCatalog.dotnet9.Tests.Catalogs;
using makeITeasy.CarCatalog.dotnet9.Tests.TestsSetup;

using Xunit;

namespace makeITeasy.CarCatalog.dotnet9.Tests
{
    public class RowVersion_Tests(DatabaseEngineFixture databaseEngineFixture) : UnitTestAutofacService(databaseEngineFixture)
    {
        [Fact]
        public async Task CreateAndGet_BasicRowVersionTest()
        {
            if (DatabaseEngineFixture.CurentDatabaseType == DatabaseType.MsSql)
            {
                ICarService carService = Resolve<ICarService>();
                string suffix = TimeOnly.FromDateTime(DateTime.Now).ToString("hhmmssffff");

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
        public async Task SameObjectUpdate_RowVersionTest()
        {
            ICarService carService = Resolve<ICarService>();
            string suffix = TimeOnly.FromDateTime(DateTime.Now).ToString("hhmmssffff");

            var result = await carService.CreateAsync(CarsCatalog.CitroenC4(suffix));

            result.Result.Should().Be(CommandState.Success);

            await Task.Delay(25);

            ////var afterFirstUpdateQueryResult = await carService.QueryAsync(new BasicCarQuery() { ID = result.Entity.Id }, includeCount: true);

            ////afterFirstUpdateQueryResult.Results.First().Version.Should().BeGreaterThan(0);

            //result = await carService.UpdateAsync(afterFirstUpdateQueryResult.Results.First());

            //result.Result.Should().Be(CommandState.Success);

            //await Task.Delay(25);

            //var afterSecondUpdateQueryResult = await carService.QueryAsync(new BasicCarQuery() { ID = result.Entity.Id }, includeCount: true);

            //afterFirstUpdateQueryResult.Results.First().Version.Should().Be(afterSecondUpdateQueryResult.Results.First().Version);

            //afterSecondUpdateQueryResult.Results.First().Name = "C4";

            //result = await carService.UpdateAsync(afterSecondUpdateQueryResult.Results.First());

            //var afterThirdUpdateQueryResult = await carService.QueryAsync(new BasicCarQuery() { ID = result.Entity.Id }, includeCount: true);

            //afterThirdUpdateQueryResult.Results.First().Version.Should().BeGreaterThan(afterSecondUpdateQueryResult.Results.First().Version);

        }
    }
}

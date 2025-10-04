using AwesomeAssertions;

using makeITeasy.AppFramework.Core.Interfaces;
using makeITeasy.AppFramework.Core.Models;
using makeITeasy.CarCatalog.dotnet10.Core.Services.Interfaces;
using makeITeasy.CarCatalog.dotnet10.Core.Services.Queries.CarQueries;
using makeITeasy.CarCatalog.dotnet10.Models;
using makeITeasy.CarCatalog.dotnet10.Tests.Catalogs;
using makeITeasy.CarCatalog.dotnet10.Tests.TestsSetup;

using Xunit;

namespace makeITeasy.CarCatalog.dotnet10.Tests
{
    public class RangeOperation_Tests(DatabaseEngineFixture databaseEngineFixture) : UnitTestAutofacService(databaseEngineFixture)
    {
        [Fact]
        public async Task BasicRangeCreation_Test()
        {
            ICarService carService = Resolve<ICarService>();
            string suffix = TimeOnly.FromDateTime(DateTime.Now).ToString("hhmmssfffffff");

            (await carService.QueryAsync(new BasicCarQuery() { NameSuffix = suffix })).Results.Should().BeEmpty();

            var createResult = await carService.CreateRangeAsync([CarsCatalog.CitroenC4(suffix), CarsCatalog.CitroenC5(suffix)]);

            createResult.Should().Match(x => x.All(y => y.Result == AppFramework.Core.Commands.CommandState.Success));

            (await carService.QueryAsync(new BasicCarQuery() { NameSuffix = suffix})).Results.Should().HaveCount(2);
        }

        [Fact]
        public async Task BasicRangeCreationWithOneError_Test()
        {
            ICarService carService = Resolve<ICarService>();
            string suffix = TimeOnly.FromDateTime(DateTime.Now).ToString("hhmmssfffffff");

            (await carService.QueryAsync(new BasicCarQuery() { NameSuffix = suffix })).Results.Should().BeEmpty();

            Car citroenC4 = CarsCatalog.CitroenC4(suffix);
            Car citroenC5 = CarsCatalog.CitroenC5(suffix);
            citroenC4.Name = citroenC5.Name;

            Func<Task> action = async () => await carService.CreateRangeAsync([citroenC4, citroenC5]);

            action.Should().ThrowAsync<Exception>();

            IList<Car> dbQueryResult = (await carService.QueryAsync(new BasicCarQuery() { NameSuffix = suffix, Includes = [x => x.Brand] })).Results;

            dbQueryResult.Should().BeEmpty();
        }

        [Fact]
        public async Task RangeCreationWithInvalidObject_Test()
        {
            ICarService carService = Resolve<ICarService>();
            string suffix = TimeOnly.FromDateTime(DateTime.Now).ToString("hhmmssfffffff");

            (await carService.QueryAsync(new BasicCarQuery() { NameSuffix = suffix })).Results.Should().BeEmpty();

            Car citroenC4 = CarsCatalog.CitroenC4(suffix);
            Car citroenC5 = CarsCatalog.CitroenC5(suffix);
            citroenC4.Name = "C";

            var createResult = await carService.CreateRangeAsync([citroenC4, citroenC5]);

            createResult.Should().Match(x => x.Count(y => y.Result == AppFramework.Core.Commands.CommandState.Success) == 1)
                .And.Match(x => x.Count(y => y.Result == AppFramework.Core.Commands.CommandState.Error) == 1)
                ;

            var queryResult = await carService.QueryAsync(new BasicCarQuery() { NameSuffix = suffix});

            queryResult.Results.Should().HaveCount(1);
        }

        [Fact]
        public async Task UpdateRange_BasicTest()
        {
            ICarService carService = Resolve<ICarService>();
            string suffix = TimeOnly.FromDateTime(DateTime.Now).ToString("hhmmssfffffff");

            var dbCreation = await carService.CreateRangeAsync([CarsCatalog.CitroenC4(suffix), CarsCatalog.CitroenC5(suffix)]);

            await carService.UpdateRangeAsync(x => x.Id > 0, new UpdateDefinition<Car>().Set(x => x.CarType, CarType.Hatchback).Set(x => x.ReleaseYear, x => x.ReleaseYear + 1000));

            var queryResult = await carService.QueryAsync(new BasicCarQuery() { NameSuffix = suffix });

            queryResult.Results.Should().Match(x => x.All(y => y.CarType == CarType.Hatchback)).And.Match(x => x.All(y => y.ReleaseYear > 3100));
        }
    }
}

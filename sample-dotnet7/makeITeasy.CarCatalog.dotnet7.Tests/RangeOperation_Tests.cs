using FluentAssertions;
using makeITeasy.CarCatalog.dotnet7.Tests.Catalogs;
using makeITeasy.CarCatalog.dotnet7.Infrastructure.Data;
using makeITeasy.CarCatalog.dotnet7.Models;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Xunit;
using makeITeasy.CarCatalog.dotnet7.Core.Services.Interfaces;
using makeITeasy.CarCatalog.dotnet7.Core.Services.Queries.CarQueries;

namespace makeITeasy.CarCatalog.dotnet7.Tests
{
    public class RangeOperation_Tests : UnitTestAutofacService<ServiceRegistrationAutofacModule>
    {
        private ICarService carService;
        private readonly List<Car> carList;

        public RangeOperation_Tests()
        {
            carService = Resolve<ICarService>();
            var t = Resolve<CarCatalogContext>();

            t.Database.EnsureCreated();

            carList = TestCarsCatalog.GetCars();
        }

        ~RangeOperation_Tests()
        {
            carService = null;
        }

        [Fact]
        public async Task BasicRangeCreation_Test()
        {
            (await carService.QueryAsync(new BaseCarQuery())).Results.Should().BeEmpty();

            var createResult = await carService.CreateRangeAsync(carList);

            createResult.Should().Match(x => x.All(y => y.Result == AppFramework.Core.Commands.CommandState.Success));

            (await carService.QueryAsync(new BaseCarQuery())).Results.Should().HaveCount(carList.Count);
        }

        [Fact]
        public async Task BasicRangeCreationWithOneError_Test()
        {
            (await carService.QueryAsync(new BaseCarQuery())).Results.Should().BeEmpty();

            carList[^1].Name = carList[0].Name;

            Func<Task> action = async () => await carService.CreateRangeAsync(carList);

            action.Should().ThrowAsync<Exception>();

            IList<Car> dbQueryResult = (await carService.QueryAsync(new BaseCarQuery() { Includes = new List<System.Linq.Expressions.Expression<Func<Car, object>>>() { x => x.Brand } })).Results;

            dbQueryResult.Should().BeEmpty();
        }

        [Fact]
        public async Task RangeCreationWithInvalidObject_Test()
        {
            (await carService.QueryAsync(new BaseCarQuery())).Results.Should().BeEmpty();

            carList[0].Name = "A";

            var createResult = await carService.CreateRangeAsync(carList);

            createResult.Should().Match(x => x.Count(y => y.Result == AppFramework.Core.Commands.CommandState.Success) == carList.Count - 1)
                .And.Match(x => x.Count(y => y.Result == AppFramework.Core.Commands.CommandState.Error) == 1)
                ;

            var queryResult = await carService.QueryAsync(new BaseCarQuery());

            queryResult.Results.Should().HaveCount(carList.Count - 1);
        }

        [Fact]
        public async Task UpdateRange_BasicTest()
        {
            var carList = TestCarsCatalog.GetCars();

            var dbCreation = await carService.CreateRangeAsync(carList);

            var udbUpdate = await carService.UpdateRangeAsync(x => x.Id > 0, x => new Car { Name = x.Name + "XX" });

            var queryResult = await carService.QueryAsync(new BaseCarQuery());

            queryResult.Results.Should().Match(x => x.All(y => y.Name.EndsWith("XX")));
        }
    }
}

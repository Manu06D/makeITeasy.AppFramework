using FluentAssertions;
using makeITeasy.CarCatalog.Core.Services.Interfaces;
using makeITeasy.CarCatalog.Core.Services.Queries.CarQueries;
using makeITeasy.CarCatalog.Infrastructure.Data;
using makeITeasy.CarCatalog.Models;
using makeITeasy.CarCatalog.Tests.Catalogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace makeITeasy.CarCatalog.Tests
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

            action.Should().Throw<Exception>();

            IList<Car> dbQueryResult = (await carService.QueryAsync(new BaseCarQuery() { Includes = new List<System.Linq.Expressions.Expression<Func<Car, object>>>() {x => x.Brand } })).Results;

            dbQueryResult.Should().BeEmpty();
        }

        [Fact]
        public async Task RangeCreationWithInvalidObject_Test()
        {
            (await carService.QueryAsync(new BaseCarQuery())).Results.Should().BeEmpty();

            carList[0].Name = "A";
            
            var createResult = await carService.CreateRangeAsync(carList);

            createResult.Should().Match(x => x.Count(y => y.Result == AppFramework.Core.Commands.CommandState.Success) == carList.Count -1)
                .And.Match(x => x.Count(y => y.Result == AppFramework.Core.Commands.CommandState.Error) ==  1)
                ;

            var queryResult = await carService.QueryAsync(new BaseCarQuery());

            queryResult.Results.Should().HaveCount(carList.Count - 1);
        }

        [Fact]
        public async Task UpdateRange_BasicTest()
        {
            var carList = TestCarsCatalog.GetCars();

            var dbCreation = await carService.CreateRangeAsync(carList);

            carList.ForEach(x => x.Name += "XXXX");

            var udbUpdate = await carService.UpdateRangeAsync(carList);
        }
    }
}

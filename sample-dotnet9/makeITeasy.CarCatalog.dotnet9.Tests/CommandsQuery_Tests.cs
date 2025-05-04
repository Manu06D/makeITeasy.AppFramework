using FluentAssertions;

using makeITeasy.AppFramework.Core.Commands;
using makeITeasy.AppFramework.Core.Queries;
using makeITeasy.CarCatalog.dotnet9.Core.Services.Interfaces;
using makeITeasy.CarCatalog.dotnet9.Core.Services.Queries.CarQueries;
using makeITeasy.CarCatalog.dotnet9.Tests.Catalogs;
using makeITeasy.CarCatalog.dotnet9.Models;

using MediatR;

using Xunit;
using makeITeasy.AppFramework.Core.Interfaces;
using makeITeasy.CarCatalog.dotnet9.Tests.TestsSetup;

namespace makeITeasy.CarCatalog.dotnet9.Tests
{
    public class CommandsQuery_Tests(DatabaseEngineFixture databaseEngineFixture) : UnitTestAutofacService(databaseEngineFixture)
    {
        [Fact]
        public async Task GenericQueryCommand_BasicTest()
        {
            IMediator _mediator = Resolve<IMediator>();

            var carService = Resolve<ICarService>();
            CommandResult<Car> newCarResult = await carService.CreateAsync(CarsCatalog.GetCars().First());

            newCarResult.Result.Should().Be(CommandState.Success);

            QueryResult<Car> result = await _mediator.Send(new GenericQueryCommand<Car>(new BasicCarQuery() { ID = newCarResult.Entity.Id }));

            result.Results.Count.Should().Be(1);
        }

        [Fact]
        public async Task GenericCreateCommand_BasicTest()
        {
            IMediator _mediator = Resolve<IMediator>();

            Car car = CarsCatalog.GetCars().First();

            CommandResult<Car> createResult = await _mediator.Send(new CreateEntityCommand<Car>(car));

            createResult.Result.Should().Be(CommandState.Success);
            createResult.Entity.Id.Should().BePositive();

            QueryResult<Car> searchResult = await _mediator.Send(new GenericQueryCommand<Car>(new BasicCarQuery() { ID = createResult.Entity.Id }));
            searchResult.Results.Should().HaveCount(1);
            searchResult.Results[0].Name.Should().Be(car.Name);
        }

        [Fact]
        public async Task GenericUpdateCommand_BasicTest()
        {
            IMediator _mediator = Resolve<IMediator>();

            var carService = Resolve<ICarService>();
            CommandResult<Car> newCarResult = await carService.CreateAsync(CarsCatalog.GetCars().First());

            newCarResult.Result.Should().Be(CommandState.Success);

            Car car = newCarResult.Entity;
            car.Name = $"car.Name{DateTime.Now.Ticks}";

            CommandResult<Car> updateResult = await _mediator.Send(new UpdateEntityCommand<Car>(car));
            updateResult.Result.Should().Be(CommandState.Success);

            QueryResult<Car> searchResult = await _mediator.Send(new GenericQueryCommand<Car>(new BasicCarQuery() { ID = car.Id }));
            searchResult.Results.Should().HaveCount(1);
            searchResult.Results[0].Name.Should().Be(car.Name);
        }

        [Fact]
        public async Task GenericFindUniqueCommand_BasicTest()
        {
            IMediator _mediator = Resolve<IMediator>();

            var carService = Resolve<ICarService>();
            CommandResult<Car> newCarResult = await carService.CreateAsync(CarsCatalog.GetCars().First(x => x.Name == "A3"));

            Car searchResult = await _mediator.Send(new GenericFindUniqueCommand<Car>(new BasicCarQuery() { Name = "A3" }));
            searchResult.Should().NotBeNull();
            searchResult.Name.Should().Be("A3");

            searchResult = await _mediator.Send(new GenericFindUniqueCommand<Car>(new BasicCarQuery() { Name = "XX" }));
            searchResult.Should().BeNull();
        }

        public class SmallCarInfo : IMapFrom<Car>
        {
            public string Name { get; set; }
        }

        [Fact]
        public async Task GenericFindUniqueCommandWithProject_BasicTest()
        {
            IMediator _mediator = Resolve<IMediator>();

            var carService = Resolve<ICarService>();
            CommandResult<Car> newCarResult = await carService.CreateAsync(CarsCatalog.GetCars().First(x => x.Name == "A3"));

            SmallCarInfo searchResult = await _mediator.Send(new GenericFindUniqueWithProjectCommand<Car, SmallCarInfo>(new BasicCarQuery() { Name = "A3" }));
            searchResult.Should().NotBeNull();
            searchResult.Name.Should().Be("A3");

            searchResult = await _mediator.Send(new GenericFindUniqueWithProjectCommand<Car, SmallCarInfo>(new BasicCarQuery() { Name = "XX" }));
            searchResult.Should().BeNull();
        }
    }
}

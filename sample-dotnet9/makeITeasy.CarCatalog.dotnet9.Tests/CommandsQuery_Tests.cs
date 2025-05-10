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
            ICarService carService = Resolve<ICarService>();
            string suffix = TimeOnly.FromDateTime(DateTime.Now).ToString("hhmmssffff");

            CommandResult<Car> newCarResult = await carService.CreateAsync(CarsCatalog.CitroenC4(suffix));

            newCarResult.Result.Should().Be(CommandState.Success);

            QueryResult<Car> result = await _mediator.Send(new GenericQueryCommand<Car>(new BasicCarQuery() { ID = newCarResult!.Entity!.Id }), TestContext.Current.CancellationToken);

            result.Results.Count.Should().Be(1);
            result.Results[0].Name.Should().Be(newCarResult.Entity.Name);
        }

        [Fact]
        public async Task GenericCreateCommand_BasicTest()
        {
            IMediator _mediator = Resolve<IMediator>();
            ICarService carService = Resolve<ICarService>();
            string suffix = TimeOnly.FromDateTime(DateTime.Now).ToString("hhmmssffff");

            CommandResult<Car> createResult = await _mediator.Send(new CreateEntityCommand<Car>(CarsCatalog.CitroenC4(suffix)), TestContext.Current.CancellationToken);

            createResult.Result.Should().Be(CommandState.Success);
            createResult.Entity.Id.Should().BePositive();

            QueryResult<Car> searchResult = await _mediator.Send(new GenericQueryCommand<Car>(new BasicCarQuery() { ID = createResult.Entity.Id }), TestContext.Current.CancellationToken);
            searchResult.Results.Should().HaveCount(1);
            searchResult.Results[0].Name.Should().Be(createResult.Entity.Name);
        }

        [Fact]
        public async Task GenericUpdateCommand_BasicTest()
        {
            IMediator _mediator = Resolve<IMediator>();
            ICarService carService = Resolve<ICarService>();
            string suffix = TimeOnly.FromDateTime(DateTime.Now).ToString("hhmmssffff");

            CommandResult<Car> newCarResult = await carService.CreateAsync(CarsCatalog.CitroenC4(suffix));

            newCarResult.Result.Should().Be(CommandState.Success);

            Car? car = newCarResult.Entity;
            car.Should().NotBeNull();
            car!.Name = $"car.Name{DateTime.Now.Ticks}";

            CommandResult<Car> updateResult = await _mediator.Send(new UpdateEntityCommand<Car>(car), TestContext.Current.CancellationToken);
            updateResult.Result.Should().Be(CommandState.Success);

            QueryResult<Car> searchResult = await _mediator.Send(new GenericQueryCommand<Car>(new BasicCarQuery() { ID = car.Id }), TestContext.Current.CancellationToken);
            searchResult.Results.Should().HaveCount(1);
            searchResult.Results[0].Name.Should().Be(car.Name);
        }

        [Fact]
        public async Task GenericFindUniqueCommand_BasicTest()
        {
            IMediator _mediator = Resolve<IMediator>();
            ICarService carService = Resolve<ICarService>();
            string suffix = TimeOnly.FromDateTime(DateTime.Now).ToString("hhmmssffff");

            CommandResult<Car> dbCreationResult = await carService.CreateAsync(CarsCatalog.CitroenC4(suffix));
            dbCreationResult.Result.Should().Be(CommandState.Success);

            Car searchResult = await _mediator.Send(new GenericFindUniqueCommand<Car>(new BasicCarQuery() { NameSuffix = suffix }), TestContext.Current.CancellationToken);
            searchResult.Should().NotBeNull();
            searchResult.Name.Should().StartWith("C4");

            searchResult = await _mediator.Send(new GenericFindUniqueCommand<Car>(new BasicCarQuery() { Name = "XX" + suffix}), TestContext.Current.CancellationToken);
            searchResult.Should().BeNull();
        }

        public class SmallCarInfo : IMapFrom<Car>
        {
            public string? Name { get; set; }
        }

        [Fact]
        public async Task GenericFindUniqueCommandWithProject_BasicTest()
        {
            IMediator _mediator = Resolve<IMediator>();
            ICarService carService = Resolve<ICarService>();
            string suffix = TimeOnly.FromDateTime(DateTime.Now).ToString("hhmmssffff");
            _ = await carService.CreateAsync(CarsCatalog.CitroenC4(suffix));

            SmallCarInfo searchResult = await _mediator.Send(new GenericFindUniqueWithProjectCommand<Car, SmallCarInfo>(new BasicCarQuery() { NameSuffix = suffix }), TestContext.Current.CancellationToken);
            searchResult.Should().NotBeNull();
            searchResult.Name.Should().Be("C4" + suffix);

            searchResult = await _mediator.Send(new GenericFindUniqueWithProjectCommand<Car, SmallCarInfo>(new BasicCarQuery() { Name = "XX" + suffix }), TestContext.Current.CancellationToken);
            searchResult.Should().BeNull();
        }
    }
}

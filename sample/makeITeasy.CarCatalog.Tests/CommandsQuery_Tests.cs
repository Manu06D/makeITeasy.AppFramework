using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using makeITeasy.AppFramework.Core.Commands;
using makeITeasy.AppFramework.Core.Queries;
using makeITeasy.CarCatalog.Core.Services.Interfaces;
using makeITeasy.CarCatalog.Core.Services.Queries.CarQueries;
using makeITeasy.CarCatalog.Infrastructure.Data;
using makeITeasy.CarCatalog.Models;
using makeITeasy.CarCatalog.Tests.Catalogs;
using MediatR;
using Xunit;

namespace makeITeasy.CarCatalog.Tests
{
    public class CommandsQuery_Tests : UnitTestAutofacService<ServiceRegistrationAutofacModule>
    {
        private IMediator _mediator;

        public CommandsQuery_Tests()
        {
            _mediator = Resolve<IMediator>();
            var dataContext = Resolve<CarCatalogContext>();

            dataContext.Database.EnsureCreated();
        }

        ~CommandsQuery_Tests()
        {
            _mediator = null;
        }

        [Fact]
        public async Task GenericQueryCommand_BasicTest()
        {
            var carService = Resolve<ICarService>();
            CommandResult<Car> newCarResult = await carService.CreateAsync(TestCarsCatalog.GetCars().First());

            newCarResult.Result.Should().Be(CommandState.Success);

            QueryResult<Car> result = await _mediator.Send(new GenericQueryCommand<Car>(new BaseCarQuery() { ID = newCarResult.Entity.Id}));

            result.Results.Count.Should().Be(1);
        }

        [Fact]
        public async Task GenericCreateCommand_BasicTest()
        {
            Car car = TestCarsCatalog.GetCars().First();

            CommandResult<Car> createResult = await _mediator.Send(new CreateEntityCommand<Car>(car));

            createResult.Result.Should().Be(CommandState.Success);
            createResult.Entity.Id.Should().BePositive();

            QueryResult<Car> searchResult = await _mediator.Send(new GenericQueryCommand<Car>(new BaseCarQuery() { ID = createResult.Entity.Id }));
            searchResult.Results.Should().HaveCount(1);
            searchResult.Results[0].Name.Should().Be(car.Name);
        }

        [Fact]
        public async Task GenericUpdateCommand_BasicTest()
        {
            var carService = Resolve<ICarService>();
            CommandResult<Car> newCarResult = await carService.CreateAsync(TestCarsCatalog.GetCars().First());

            newCarResult.Result.Should().Be(CommandState.Success);
            
            Car car = newCarResult.Entity;
            car.Name = $"car.Name{DateTime.Now.Ticks}";

            CommandResult<Car> updateResult = await _mediator.Send(new UpdateEntityCommand<Car>(car));
            updateResult.Result.Should().Be(CommandState.Success);

            QueryResult<Car> searchResult = await _mediator.Send(new GenericQueryCommand<Car>(new BaseCarQuery() { ID = car.Id }));
            searchResult.Results.Should().HaveCount(1);
            searchResult.Results[0].Name.Should().Be(car.Name);
        }
    }
}

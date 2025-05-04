using FluentAssertions;

using makeITeasy.AppFramework.Core.Commands;
using makeITeasy.AppFramework.Core.Queries;
using makeITeasy.CarCatalog.dotnet9.Core.Services.Queries.CarQueries;
using makeITeasy.CarCatalog.dotnet9.Models;
using makeITeasy.CarCatalog.dotnet9.Tests.Catalogs;
using makeITeasy.CarCatalog.dotnet9.Tests.TestsSetup;

using MediatR;

using Xunit;

namespace makeITeasy.CarCatalog.dotnet9.Tests
{
    public class MediaTrCommand_Tests(DatabaseEngineFixture databaseEngineFixture) : UnitTestAutofacService(databaseEngineFixture)
    {
        [Fact]
        public async Task CreateAndUpdateCommand_BasicTest()
        {
            IMediator mediator = Resolve<IMediator>();
            string suffix = TimeOnly.FromDateTime(DateTime.Now).ToString("hhmmssffff");
            Car newCar = CarsCatalog.CitroenC4(suffix);

            var resultCreate = await mediator.Send(new CreateEntityCommand<Car>(newCar), TestContext.Current.CancellationToken);
            resultCreate.Result.Should().Be(CommandState.Success);
            newCar.Id.Should().BeGreaterThan(0);
            newCar.Name.Should().EndWith(suffix);

            newCar.Name += "Update";

            var resultUpdate = await mediator.Send(new UpdatePartialEntityCommand<Car>(newCar, [nameof(newCar.Name)]), TestContext.Current.CancellationToken);
            resultUpdate.Result.Should().Be(CommandState.Success);

            var query = await mediator.Send(new GenericQueryCommand<Car>(new BasicCarQuery() { ID = newCar.Id }), TestContext.Current.CancellationToken);
            query.Results[0].Name.Should().EndWith(suffix + "Update");

            var mediatorLog = Resolve<MediatRLog>();
            mediatorLog.Counter.Should().Be(4);
        }

        [Fact]
        public async Task CreateAndDeleteCommand_BasicTest()
        {
            IMediator mediator = Resolve<IMediator>();
            string suffix = TimeOnly.FromDateTime(DateTime.Now).ToString("hhmmssffff");
            Car car = CarsCatalog.CitroenC4(suffix);

            var resultCreate = await mediator.Send(new CreateEntityCommand<Car>(car), TestContext.Current.CancellationToken);
            resultCreate.Result.Should().Be(CommandState.Success);

            car.Id.Should().BeGreaterThan(0);
            car.Name.Should().EndWith(suffix);

            var clonedCar = TestHelper.Clone(car);

            var resultUpdate = await mediator.Send(new DeleteEntityCommand<Car>(clonedCar), TestContext.Current.CancellationToken);
            resultUpdate.Result.Should().Be(CommandState.Success);

            var query = await mediator.Send(new GenericQueryCommand<Car>(new BasicCarQuery() { ID = car.Id }), TestContext.Current.CancellationToken);
            query.Results.FirstOrDefault().Should().BeNull();

            var mediatorLog = Resolve<MediatRLog>();
            mediatorLog.Counter.Should().Be(4);
        }
    }
}

using FluentAssertions;

using makeITeasy.AppFramework.Core.Commands;
using makeITeasy.AppFramework.Core.Queries;
using makeITeasy.AppFramework.Models;
using makeITeasy.CarCatalog.dotnet9.Core.Services.Interfaces;
using makeITeasy.CarCatalog.dotnet9.Core.Services.Queries.CarQueries;
using makeITeasy.CarCatalog.dotnet9.Tests.Catalogs;
using makeITeasy.CarCatalog.dotnet9.Models;

using MediatR;

using Xunit;
using makeITeasy.CarCatalog.dotnet9.Tests.TestsSetup;

namespace makeITeasy.CarCatalog.dotnet9.Tests
{
    public class QueryWithValidation_Tests(DatabaseEngineFixture databaseEngineFixture) : UnitTestAutofacService(databaseEngineFixture)
    {
        public class BaseCarQueryWithValidation : BasicCarQuery, IIsValidSpecification
        {
            public bool IsValid()
            {
                return ID >= 0 || Name?.Length > 0;
            }
        }

        [Fact]
        public async Task GenericQueryCommand_BasicTest()
        {
            ICarService carService = Resolve<ICarService>();
            IMediator mediator= Resolve<IMediator>();
            string suffix = TimeOnly.FromDateTime(DateTime.Now).ToString("hhmmssffff");
            Car newCar = CarsCatalog.CitroenC4(suffix);

            CommandResult<Car> newCarResult = await carService.CreateAsync(newCar);

            QueryResult<Car> result = await mediator.Send(new GenericQueryCommand<Car>(new BaseCarQueryWithValidation() { ID = newCarResult.Entity.Id }), TestContext.Current.CancellationToken);

            result.Results.Count.Should().Be(1);

            Func<Task> act = () => mediator.Send(new GenericQueryCommand<Car>(new BaseCarQueryWithValidation() { ID = -1 }));

            await act.Should().ThrowAsync<InvalidQueryException>();

            act = () => mediator.Send(new GenericQueryCommand<Car>(new BaseCarQueryWithValidation()));

            await act.Should().ThrowAsync<InvalidQueryException>();

            act = () => mediator.Send(new GenericQueryCommand<Car>(new BaseCarQueryWithValidation() { Name = newCarResult.Entity.Name }));
            result.Results.Count.Should().Be(1);
        }
    }
}

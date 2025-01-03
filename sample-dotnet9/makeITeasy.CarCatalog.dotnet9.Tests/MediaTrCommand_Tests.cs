﻿using FluentAssertions;

using makeITeasy.AppFramework.Core.Commands;
using makeITeasy.AppFramework.Core.Queries;
using makeITeasy.CarCatalog.dotnet9.Core.Services.Queries.CarQueries;
using makeITeasy.CarCatalog.dotnet9.Infrastructure.Data;
using makeITeasy.CarCatalog.dotnet9.Models;

using MediatR;

using Xunit;

namespace makeITeasy.CarCatalog.dotnet9.Tests
{
    public class MediaTrCommand_Tests : UnitTestAutofacService<ServiceRegistrationAutofacModule>
    {
        private IMediator _mediator;

        public MediaTrCommand_Tests()
        {
            _mediator = Resolve<IMediator>();
            var dataContext = Resolve<CarCatalogContext>();

            dataContext.Database.EnsureCreated();
        }

        ~MediaTrCommand_Tests()
        {
            _mediator = null;
        }

        [Fact]
        public async Task CreateAndUpdateCommand_BasicTest()
        {
            Car newCar = new ()
            {
                Name = "C3",
                ReleaseYear = 2011,
                Brand = new Brand()
                {
                    Name = "Citroen",
                    Country = new Country()
                    {
                        Name = "France",
                        CountryCode = "FR"
                    }
                }
            };

            var resultCreate = await _mediator.Send(new CreateEntityCommand<Car>(newCar));
            resultCreate.Result.Should().Be(CommandState.Success);
            newCar.Id.Should().BeGreaterThan(0);
            newCar.Name.Should().Be("C3");

            newCar.Name = "C4";

            var resultUpdate = await _mediator.Send(new UpdatePartialEntityCommand<Car>(newCar, [nameof(newCar.Name)]));
            resultUpdate.Result.Should().Be(CommandState.Success);

            var query = await _mediator.Send(new GenericQueryCommand<Car>(new BasicCarQuery() { ID = newCar.Id }));
            query.Results[0].Name.Should().Be("C4");

            var mediatorLog = Resolve<MediatRLog>();
            mediatorLog.Counter.Should().Be(4);
        }

        [Fact]
        public async Task CreateAndDeleteCommand_BasicTest()
        {
            Car car = new()
            {
                Name = "C3",
                ReleaseYear = 2011,
                Brand = new Brand()
                {
                    Name = "Citroen",
                    Country = new Country()
                    {
                        Name = "France",
                        CountryCode = "FR"
                    }
                }
            };

            var resultCreate = await _mediator.Send(new CreateEntityCommand<Car>(car));
            resultCreate.Result.Should().Be(CommandState.Success);

            car.Id.Should().BeGreaterThan(0);
            car.Name.Should().Be("C3");

            var clonedCar = TestHelper.Clone(car);

            var resultUpdate = await _mediator.Send(new DeleteEntityCommand<Car>(clonedCar));
            resultUpdate.Result.Should().Be(CommandState.Success);

            var query = await _mediator.Send(new GenericQueryCommand<Car>(new BasicCarQuery() { ID = car.Id }));
            query.Results.FirstOrDefault().Should().BeNull();

            var mediatorLog = Resolve<MediatRLog>();
            mediatorLog.Counter.Should().Be(4);
        }
    }
}

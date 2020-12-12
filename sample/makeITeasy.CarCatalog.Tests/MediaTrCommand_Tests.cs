using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using makeITeasy.AppFramework.Core.Commands;
using makeITeasy.AppFramework.Core.Queries;
using makeITeasy.CarCatalog.Core.Services.Queries.CarQueries;
using makeITeasy.CarCatalog.Infrastructure.Data;
using makeITeasy.CarCatalog.Models;
using MediatR;
using Xunit;

namespace makeITeasy.CarCatalog.Tests
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
            Car newCar = new Car()
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

            var resultUpdate = await _mediator.Send(new UpdatePartialEntityCommand<Car>(newCar, new string[] { nameof(newCar.Name)}));
            resultUpdate.Result.Should().Be(CommandState.Success);

            var query = await _mediator.Send(new GenericQueryCommand<Car>(new BaseCarQuery() { ID = newCar.Id }));
            query.Results.First().Name.Should().Be("C4");
        }
    }
}

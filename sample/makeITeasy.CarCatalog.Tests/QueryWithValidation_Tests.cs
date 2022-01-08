using FluentAssertions;

using makeITeasy.AppFramework.Core.Commands;
using makeITeasy.AppFramework.Core.Queries;
using makeITeasy.AppFramework.Models;
using makeITeasy.CarCatalog.Core.Services.Interfaces;
using makeITeasy.CarCatalog.Core.Services.Queries.CarQueries;
using makeITeasy.CarCatalog.Infrastructure.Data;
using makeITeasy.CarCatalog.Models;

using makeITeasy.CarCatalog.Tests.Catalogs;

using MediatR;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xunit;

namespace makeITeasy.CarCatalog.Tests
{
    public class QueryWithValidation_Tests : UnitTestAutofacService<ServiceRegistrationAutofacModule>
    {
        private IMediator _mediator;
        public QueryWithValidation_Tests()
        {
            _mediator = Resolve<IMediator>();
            var dataContext = Resolve<CarCatalogContext>();

            dataContext.Database.EnsureCreated();
        }

        ~QueryWithValidation_Tests()
        {
            _mediator = null;
        }

        public class BaseCarQueryWithValidation : BaseCarQuery, IIsValidSpecification
        {
            public bool IsValid()
            {
                return ID >= 0 || Name?.Length > 0;
            }
        }

        [Fact]
        public async Task GenericQueryCommand_BasicTest()
        {
            var carService = Resolve<ICarService>();
            CommandResult<Car> newCarResult = await carService.CreateAsync(TestCarsCatalog.GetCars().First());

            QueryResult<Car> result = await _mediator.Send(new GenericQueryCommand<Car>(new BaseCarQueryWithValidation() { ID = newCarResult.Entity.Id }));

            result.Results.Count.Should().Be(1);

            Func<Task> act = () => _mediator.Send(new GenericQueryCommand<Car>(new BaseCarQueryWithValidation() { ID = -1}));

            await act.Should().ThrowAsync<InvalidQueryException>();

            act = () => _mediator.Send(new GenericQueryCommand<Car>(new BaseCarQueryWithValidation() {}));

            await act.Should().ThrowAsync<InvalidQueryException>();

            act = () => _mediator.Send(new GenericQueryCommand<Car>(new BaseCarQueryWithValidation() { Name = newCarResult.Entity.Name}));

            result.Results.Count.Should().Be(1);
        }
    }
}

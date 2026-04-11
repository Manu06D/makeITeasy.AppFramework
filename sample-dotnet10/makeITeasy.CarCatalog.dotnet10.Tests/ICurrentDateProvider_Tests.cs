using Autofac;

using AwesomeAssertions;

using makeITeasy.AppFramework.Core.Queries;
using makeITeasy.AppFramework.Models;
using makeITeasy.CarCatalog.dotnet10.Core.Services.Interfaces;
using makeITeasy.CarCatalog.dotnet10.Core.Services.Queries.CarQueries;
using makeITeasy.CarCatalog.dotnet10.Core.Services.Queries.CountryQueries;
using makeITeasy.CarCatalog.dotnet10.Infrastructure.Data;
using makeITeasy.CarCatalog.dotnet10.Models;
using makeITeasy.CarCatalog.dotnet10.Tests.Catalogs;
using makeITeasy.CarCatalog.dotnet10.Tests.TestsSetup;

using MediatR;

using Xunit;

namespace makeITeasy.CarCatalog.dotnet10.Tests
{
    public class CustomerDateTimeProvider : ICurrentDateProvider
    {
        public DateTime Now => new(2000, 12, 25);
    }

    public class ICurrentDateProvider_Tests(DatabaseFixture fixture) : AutoMockFixture(fixture)
    {
        [Fact]
        public async Task CustomDateTimeProviderWithCustomService_DateTime()
        {
            InitialiseAutoMock(cfg =>
            {
                cfg.RegisterModule(new ServiceRegistrationAutofacModule() { DatabaseConnectionString = fixture.ConnectionString, DatabaseType = GlobalTestSetup.DatabaseType });
                cfg.RegisterType<CustomerDateTimeProvider>().As<ICurrentDateProvider>();
            });

            InitDatabase<CarCatalogContext>();
            IMediator mediator = Resolve<IMediator>();

            await CarsCatalog.CreateCarsAsync(this);

            QueryResult<Car> getResult = await mediator.Send(new GenericQueryCommand<Car>(new BasicCarQuery() { NameSuffix = TestUniqueId }), TestContext.Current.CancellationToken);

            getResult.Results.Count.Should().Be(2);

            getResult.Results.All(x => x.CreationDate == new DateTime(2000, 12, 25)).Should().BeTrue();
        }

        [Fact]
        public async Task CustomerDateTimeProviderWithGenericService_DateTime()
        {
            InitialiseAutoMock(cfg =>
            {
                cfg.RegisterModule(new ServiceRegistrationAutofacModule() { DatabaseConnectionString = fixture.ConnectionString, DatabaseType = GlobalTestSetup.DatabaseType });
                cfg.RegisterType<CustomerDateTimeProvider>().As<ICurrentDateProvider>();
            });

            InitDatabase<CarCatalogContext>();
            ICountryService countryService = Resolve<ICountryService>();

            var car = await countryService.CreateAsync(new Country() { Name = "FR" + TestUniqueId, CountryCode = "FR" });

            var getResult = await countryService.QueryAsync(new BaseCountryQuery() { NameSuffix = TestUniqueId });

            getResult.Results.Count.Should().BePositive();

            getResult.Results.All(x => x.CreationDate == new DateTime(2000, 12, 25)).Should().BeTrue();
        }
    }
}

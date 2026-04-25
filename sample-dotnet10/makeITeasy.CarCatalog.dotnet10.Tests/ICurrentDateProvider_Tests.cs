using Autofac;

using AwesomeAssertions;

using makeITeasy.AppFramework.Core.Queries;
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
    public class FakeTimeProvider : TimeProvider
    {
        public override DateTimeOffset GetUtcNow()
        {
            var localDate = new DateTime(2000, 12, 25, 0, 0, 0, DateTimeKind.Local);
            return new DateTimeOffset(localDate).ToUniversalTime();
        }
    }

    public class TimeProvider_Tests(DatabaseFixture fixture) : AutoMockFixture(fixture)
    {
        [Fact]
        public async Task CustomTimeProviderWithCustomService_DateTimeXXXX()
        {
            InitialiseAutoMock(cfg =>
            {
                cfg.RegisterModule(new ServiceRegistrationAutofacModule() { DatabaseConnectionString = fixture.ConnectionString, DatabaseType = GlobalTestSetup.DatabaseType });
            });

            InitDatabase<CarCatalogContext>();
            IMediator mediator = Resolve<IMediator>();

            (_, _, _, string suffix, _) = await CarsCatalog.CreateCarsAsync(this);

            QueryResult<Car> getResult = await mediator.Send(new GenericQueryCommand<Car>(new BasicCarQuery() { NameSuffix = suffix }), TestContext.Current.CancellationToken);

            getResult.Results.Count.Should().Be(2);

            getResult.Results[0].CreationDate.Should().BeCloseTo(DateTime.Now, new TimeSpan(0, 0, 1));
        }

        [Fact]
        public async Task CustomTimeProviderWithCustomService_DateTime()
        {
            InitialiseAutoMock(cfg =>
            {
                cfg.RegisterModule(new ServiceRegistrationAutofacModule() { DatabaseConnectionString = fixture.ConnectionString, DatabaseType = GlobalTestSetup.DatabaseType });
                cfg.RegisterType<FakeTimeProvider>().As<TimeProvider>();
            });

            InitDatabase<CarCatalogContext>();
            IMediator mediator = Resolve<IMediator>();

            (_, _, _, string suffix, _) = await CarsCatalog.CreateCarsAsync(this);

            QueryResult<Car> getResult = await mediator.Send(new GenericQueryCommand<Car>(new BasicCarQuery() { NameSuffix = suffix }), TestContext.Current.CancellationToken);

            getResult.Results.Count.Should().Be(2);

            getResult.Results.All(x => x.CreationDate == new DateTime(2000, 12, 25)).Should().BeTrue();
        }

        [Fact]
        public async Task CustomTimeProviderWithGenericService_DateTime()
        {
            InitialiseAutoMock(cfg =>
            {
                cfg.RegisterModule(new ServiceRegistrationAutofacModule() { DatabaseConnectionString = fixture.ConnectionString, DatabaseType = GlobalTestSetup.DatabaseType });
                cfg.RegisterType<FakeTimeProvider>().As<TimeProvider>();
            });

            string suffix = TimeOnly.FromDateTime(DateTime.Now).ToString("hhmmssfffffff");

            InitDatabase<CarCatalogContext>();
            ICountryService countryService = Resolve<ICountryService>();

            var car = await countryService.CreateAsync(new Country() { Name = "FR" + suffix, CountryCode = "FR" });

            var getResult = await countryService.QueryAsync(new BaseCountryQuery() { NameSuffix = suffix });

            getResult.Results.Count.Should().BePositive();

            getResult.Results.All(x => x.CreationDate == new DateTime(2000, 12, 25)).Should().BeTrue();
        }
    }
}

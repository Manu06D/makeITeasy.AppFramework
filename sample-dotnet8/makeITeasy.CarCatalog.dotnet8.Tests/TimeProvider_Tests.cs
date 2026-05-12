using Autofac;

using AwesomeAssertions;

using makeITeasy.CarCatalog.dotnet8.Core.Services.Interfaces;
using makeITeasy.CarCatalog.dotnet8.Core.Services.Queries.CarQueries;
using makeITeasy.CarCatalog.dotnet8.Core.Services.Queries.CountryQueries;
using makeITeasy.CarCatalog.dotnet8.Infrastructure.Data;
using makeITeasy.CarCatalog.dotnet8.Tests.Catalogs;


using Xunit;

namespace makeITeasy.CarCatalog.dotnet8.Tests
{
    public class FakeTimeProvider : TimeProvider
    {
        public override DateTimeOffset GetUtcNow()
        {
            DateTime localDate = new(2000, 12, 25, 0, 0, 0, DateTimeKind.Local);
            return new DateTimeOffset(localDate).ToUniversalTime();
        }
    }

    public class ServiceRegistrationAutofacModuleWithDateProvider : ServiceRegistrationAutofacModule
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<FakeTimeProvider>().As<TimeProvider>();

            base.Load(builder);
        }
    }

    public class TimeProvider_Tests : UnitTestAutofacService<ServiceRegistrationAutofacModuleWithDateProvider>
    {
        public TimeProvider_Tests()
        {
            var t = Resolve<CarCatalogContext>();

            t.Database.EnsureCreated();
        }

        [Fact]
        public async Task CustomerDateTimeProviderWithCustomerService_DateTime()
        {
            ICarService carService = Resolve<ICarService>();

            var car = await carService.CreateAsync(TestCarsCatalog.GetCars().First());

            var getResult = await carService.QueryAsync(new BaseCarQuery() { });

            getResult.Results.Count.Should().BePositive();

            getResult.Results.All(x => x.CreationDate == new DateTime(2000, 12, 25)).Should().BeTrue();
        }

        [Fact]
        public async Task CustomerDateTimeProviderWithGenericService_DateTime()
        {
            ICountryService countryService = Resolve<ICountryService>();

            var car = await countryService.CreateAsync(TestCarsCatalog.GetCars().First().Brand.Country);

            var getResult = await countryService.QueryAsync(new BaseCountryQuery() { });

            getResult.Results.Count.Should().BePositive();

            getResult.Results.All(x => x.CreationDate == new DateTime(2000, 12, 25)).Should().BeTrue();
        }
    }
}

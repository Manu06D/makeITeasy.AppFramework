using Autofac;

using AwesomeAssertions;

using makeITeasy.AppFramework.Models;
using makeITeasy.CarCatalog.dotnet9.Core.Services.Interfaces;
using makeITeasy.CarCatalog.dotnet9.Core.Services.Queries.CarQueries;
using makeITeasy.CarCatalog.dotnet9.Core.Services.Queries.CountryQueries;
using makeITeasy.CarCatalog.dotnet9.Models;
using makeITeasy.CarCatalog.dotnet9.Tests.TestsSetup;

using Xunit;

namespace makeITeasy.CarCatalog.dotnet9.Tests
{
    public class CustomerDateTimeProvider : ICurrentDateProvider
    {
        public DateTime Now => new(2000, 12, 25);
    }

    public class CustomerDateTimeProviderModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<CustomerDateTimeProvider>().As<ICurrentDateProvider>();

            base.Load(builder);
        }
    }

    public class ICurrentDateProvider_Tests(DatabaseEngineFixture databaseEngineFixture) : UnitTestAutofacService(databaseEngineFixture, [typeof(CustomerDateTimeProviderModule)])
    {
        [Fact]
        public async Task CustomDateTimeProviderWithCustomService_DateTime()
        {
            (ICarService carService, IBrandService brandService, Brand citroenBrand, string suffix, _) = await CreateCarsAsync();

            var getResult = await carService.QueryAsync(new BasicCarQuery() { NameSuffix = suffix});

            getResult.Results.Count.Should().Be(2);

            getResult.Results.All(x => x.CreationDate == new DateTime(2000, 12, 25)).Should().BeTrue();
        }

        [Fact]
        public async Task CustomerDateTimeProviderWithGenericService_DateTime()
        {
            ICountryService countryService = Resolve<ICountryService>();
            string suffix = TimeOnly.FromDateTime(DateTime.Now).ToString("hhmmssfffffff");

            var car = await countryService.CreateAsync(new Country() { Name = "FR" + suffix, CountryCode = "FR" });

            var getResult = await countryService.QueryAsync(new BaseCountryQuery() { NameSuffix = suffix });

            getResult.Results.Count.Should().BePositive();

            getResult.Results.All(x => x.CreationDate == new DateTime(2000, 12, 25)).Should().BeTrue();
        }
    }
}

using Autofac;

using FluentAssertions;

using makeITeasy.AppFramework.Models;
using makeITeasy.CarCatalog.dotnet8.Core.Services.Interfaces;
using makeITeasy.CarCatalog.dotnet8.Core.Services.Queries.BrandQueries;
using makeITeasy.CarCatalog.dotnet8.Core.Services.Queries.CarQueries;
using makeITeasy.CarCatalog.dotnet8.Core.Services.Queries.CountryQueries;
using makeITeasy.CarCatalog.dotnet8.Infrastructure.Data;
using makeITeasy.CarCatalog.dotnet8.Tests.Catalogs;

using System;
using System.Linq;
using System.Threading.Tasks;

using Xunit;

namespace makeITeasy.CarCatalog.dotnet8.Tests
{
    public class CustomerDateTimeProvider : ICurrentDateProvider
    {
        public DateTime Now => new(2000, 12, 25);
    }

    public class ServiceRegistrationAutofacModuleWithDateProvider : ServiceRegistrationAutofacModule
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<CustomerDateTimeProvider>().As<ICurrentDateProvider>();

            base.Load(builder);
        }
    }

    public class ICurrentDateProvider_Tests : UnitTestAutofacService<ServiceRegistrationAutofacModuleWithDateProvider>
    {
        public ICurrentDateProvider_Tests()
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

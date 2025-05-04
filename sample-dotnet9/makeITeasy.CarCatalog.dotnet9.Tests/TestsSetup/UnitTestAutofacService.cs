using Autofac;

using makeITeasy.CarCatalog.dotnet9.Core.Services.Interfaces;
using makeITeasy.CarCatalog.dotnet9.Infrastructure.Data;
using makeITeasy.CarCatalog.dotnet9.Models;
using makeITeasy.CarCatalog.dotnet9.Tests.Catalogs;

using Testcontainers.MsSql;

using Xunit;

namespace makeITeasy.CarCatalog.dotnet9.Tests.TestsSetup
{
    public class UnitTestAutofacService : DatabaseFixture
    {
        public UnitTestAutofacService(DatabaseEngineFixture databaseEngineFixture) : base(databaseEngineFixture)
        {
        }

        public async Task<(ICarService carService, IBrandService brandService, Brand citroenBrand, string suffix)> CreateCarsAsync()
        {
            ICarService carService = Resolve<ICarService>();
            IBrandService brandService = Resolve<IBrandService>();
            ICountryService countryService = Resolve<ICountryService>();

            string suffix = TimeOnly.FromDateTime(DateTime.Now).ToString("hhmmssffff");

            Country country = CarsCatalog.France;
            await countryService.CreateAsync(country);

            Brand citroenBrand = CarsCatalog.Citroen(suffix, countryId: country.Id);
            await brandService.CreateAsync(citroenBrand);

            await carService.CreateAsync(CarsCatalog.CitroenC4(suffix, brandId: citroenBrand.Id));
            await carService.CreateAsync(CarsCatalog.CitroenC5(suffix, brandId: citroenBrand.Id));

            return (carService, brandService, citroenBrand, suffix);
        }
    }

    public class DatabaseFixture : IAsyncLifetime
    {
        protected string connectionString;
        protected IContainer? container;

        public DatabaseFixture(DatabaseEngineFixture databaseEngineFixture)
        {
            string s = "";
            DatabaseEngineFixture = databaseEngineFixture;
        }

        public DatabaseEngineFixture DatabaseEngineFixture { get; }

        public async ValueTask InitializeAsync()
        {
            //var network = new NetworkBuilder().Build();

            var msSqlContainer =
                new MsSqlBuilder()
                 //.WithNetwork(network)
                 .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
                //.WithPassword("sa")
                .WithReuse(true)
                //todo change label
                .WithLabel("reuse-id", "aaaa")
                .Build();
            await msSqlContainer.StartAsync();

            connectionString = msSqlContainer.GetConnectionString();

            var builder = new Autofac.ContainerBuilder();
            builder.RegisterModule(new ServiceRegistrationAutofacModule() { DatabaseConnectionString = connectionString });

            container = builder.Build();

            var t = Resolve<CarCatalogContext>();

            t.Database.EnsureCreated();
        }

        public async ValueTask DisposeAsync()
        {
            await container.DisposeAsync();
        }

        protected TEntity Resolve<TEntity>()
        {
            //return default();
            return container.Resolve<TEntity>();
        }
    }
}
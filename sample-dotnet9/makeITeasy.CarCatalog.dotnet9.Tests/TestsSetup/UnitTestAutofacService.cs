using Autofac;

using makeITeasy.AppFramework.Core.Commands;
using makeITeasy.CarCatalog.dotnet9.Core.Services.Interfaces;
using makeITeasy.CarCatalog.dotnet9.Infrastructure.Data;
using makeITeasy.CarCatalog.dotnet9.Models;
using makeITeasy.CarCatalog.dotnet9.Tests.Catalogs;

using Microsoft.Data.Sqlite;

using Testcontainers.MsSql;

using Xunit;

namespace makeITeasy.CarCatalog.dotnet9.Tests.TestsSetup
{
    public class UnitTestAutofacService : DatabaseFixture
    {
        public UnitTestAutofacService(DatabaseEngineFixture databaseEngineFixture, List<Type> modules = null) : base(databaseEngineFixture, modules)
        {
        }

        public async Task<(ICarService carService, IBrandService brandService, Brand citroenBrand, string suffix, List<Car> cars)> CreateCarsAsync()
        {
            ICarService carService = Resolve<ICarService>();
            IBrandService brandService = Resolve<IBrandService>();
            ICountryService countryService = Resolve<ICountryService>();

            string suffix = TimeOnly.FromDateTime(DateTime.Now).ToString("hhmmssfffffff");
            List<Car> cars = [];

            Country country = CarsCatalog.France;
            await countryService.CreateAsync(country);

            Brand citroenBrand = CarsCatalog.Citroen(suffix, countryId: country.Id);
            await brandService.CreateAsync(citroenBrand);

            Car c4car = CarsCatalog.CitroenC4(suffix, brandId: citroenBrand.Id);
            if ((await carService.CreateAsync(c4car)).Result == CommandState.Success)
            {
                cars.Add(c4car);
            }

            Car c5car = CarsCatalog.CitroenC5(suffix, brandId: citroenBrand.Id);
            if ((await carService.CreateAsync(c5car)).Result == CommandState.Success)
            {
                cars.Add(c5car);
            }

            return (carService, brandService, citroenBrand, suffix, cars);
        }
    }

    public class DatabaseFixture : IAsyncLifetime
    {
        protected string connectionString;
        protected IContainer? container;
        private readonly List<Type> modules;

        public DatabaseFixture(DatabaseEngineFixture databaseEngineFixture, List<Type> modules = null)
        {
            DatabaseEngineFixture = databaseEngineFixture;
            this.modules = modules;
        }

        public DatabaseEngineFixture DatabaseEngineFixture { get; }

        public async ValueTask InitializeAsync()
        {
            //var network = new NetworkBuilder().Build();

            if (DatabaseEngineFixture.CurentDatabaseType == DatabaseType.MsSql)
            {
                var msSqlContainer =
                    new MsSqlBuilder()
                     //.WithNetwork(network)
                     .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
                    .WithReuse(true)
                    //todo change label
                    .WithLabel("reuse-id", "makeiteasyUnitTests")
                    .Build();
                await msSqlContainer.StartAsync();

                connectionString = msSqlContainer.GetConnectionString();
            }
            else if (DatabaseEngineFixture.CurentDatabaseType == DatabaseType.SqlLite)
            {
                var sqlLiteMemoryConnection = new SqliteConnection("DataSource=:memory:");
                sqlLiteMemoryConnection.Open();

                connectionString = sqlLiteMemoryConnection.ConnectionString;
            }
            else
            {
                throw new NotSupportedException("Unsupported database type");
            }

            var builder = new ContainerBuilder();
            builder.RegisterModule(new ServiceRegistrationAutofacModule() { DatabaseConnectionString = connectionString, DatabaseType = DatabaseEngineFixture.CurentDatabaseType });
            if (modules?.Count > 0)
            {
                foreach (var module in modules)
                {
                    builder.RegisterModule(Activator.CreateInstance(module) as Module);
                }
            }
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
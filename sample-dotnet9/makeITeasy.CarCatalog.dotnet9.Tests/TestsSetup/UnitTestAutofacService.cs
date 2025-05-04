using Autofac;
using Autofac.Core;

using DotNet.Testcontainers.Builders;

using makeITeasy.CarCatalog.dotnet9.Infrastructure.Data;

using Microsoft.EntityFrameworkCore;

using Testcontainers.MsSql;

using Xunit;


namespace makeITeasy.CarCatalog.dotnet9.Tests.TestsSetup
{

    public class UnitTestAutofacService : DatabaseFixture
    {
        public UnitTestAutofacService(DatabaseEngineFixture databaseEngineFixture) : base(databaseEngineFixture)
        {
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
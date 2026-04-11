using Autofac;

using makeITeasy.AppFramework.Core.Commands;
using makeITeasy.CarCatalog.dotnet10.Core.Services.Interfaces;
using makeITeasy.CarCatalog.dotnet10.Models;
using makeITeasy.CarCatalog.dotnet10.Tests.Catalogs;
using makeITeasy.CarCatalog.dotnet10.Tests.TestsSetup;

using Microsoft.Data.Sqlite;

using Testcontainers.MsSql;

using Xunit;

[assembly: AssemblyFixture(typeof(DatabaseFixture))]
namespace makeITeasy.CarCatalog.dotnet10.Tests.TestsSetup
{
    public class DatabaseFixture : IAsyncLifetime
    {
        public string? ConnectionString;
        protected IContainer? container;

        public async ValueTask InitializeAsync()
        {
            await StartDatabaseEngine();
        }

        private async Task StartDatabaseEngine()
        {
            if (GlobalTestSetup.DatabaseType == DatabaseType.MsSql)
            {
                var msSqlContainer =
                    new MsSqlBuilder("mcr.microsoft.com/mssql/server:2022-latest")
                    .WithReuse(true)
                    .WithLabel("reuse-id", "makeiteasyUnitTests")
                    .Build();
                await msSqlContainer.StartAsync();

                ConnectionString = msSqlContainer.GetConnectionString();
            }
            else if (GlobalTestSetup.DatabaseType == DatabaseType.SqlLite)
            {
                var sqlLiteMemoryConnection = new SqliteConnection("DataSource=:memory:");
                sqlLiteMemoryConnection.Open();

                ConnectionString = sqlLiteMemoryConnection.ConnectionString;
            }
            else
            {
                throw new NotSupportedException("Unsupported database type");
            }
        }

        public async ValueTask DisposeAsync()
        {
            //dont dispose the container to allow reusing the database container
            //await Container!.DisposeAsync();
            //GC.SuppressFinalize(this);
        }
     }
}

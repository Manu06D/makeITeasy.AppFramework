using Autofac;

using makeITeasy.CarCatalog.dotnet10.Infrastructure.Data;

using Microsoft.EntityFrameworkCore;

using Xunit;

namespace makeITeasy.CarCatalog.dotnet10.Tests.TestsSetup
{
    public class AutofacFixture(DatabaseFixture fixture) : IAsyncLifetime, IResolveEntity
    {
        protected DatabaseFixture Fixture { get; } = fixture;
        protected IContainer? container;

        public async ValueTask InitializeAsync()
        {
            ContainerBuilder builder = new();

            builder.RegisterModule(new ServiceRegistrationAutofacModule() { DatabaseConnectionString = fixture.ConnectionString, DatabaseType = GlobalTestSetup.DatabaseType });

            container = builder.Build();

            InitDatabase<CarCatalogContext>();
        }

        public static string TestUniqueId => TestContext.Current?.Test.UniqueID[..10] ?? DateTime.Now.Ticks.ToString()[..10];

        public TEntity Resolve<TEntity>()
        {
            return container.Resolve<TEntity>();
        }

        public ValueTask DisposeAsync()
        {
            return ValueTask.CompletedTask;
        }

        private static readonly SemaphoreSlim semaphore = new(1);

        public void InitDatabase<T>() where T : DbContext
        {
            T dbContext = Resolve<T>();
            semaphore.Wait();
            try
            {
                dbContext.Database.EnsureCreated();
            }
            finally
            {
                semaphore.Release();
            }
        }
    }
}
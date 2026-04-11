using Autofac;
using Autofac.Extras.Moq;

using Microsoft.EntityFrameworkCore;

using Xunit;

namespace makeITeasy.CarCatalog.dotnet10.Tests.TestsSetup
{
    public class AutoMockFixture(DatabaseFixture fixture) : IAsyncLifetime, IResolveEntity
    {
        protected DatabaseFixture Fixture { get; } = fixture;
        protected AutoMock? autoMock;

        public ValueTask DisposeAsync()
        {
            autoMock?.Dispose();
            return ValueTask.CompletedTask;
        }

        public async ValueTask InitializeAsync()
        {
        }

        public void InitialiseAutoMock(Action<ContainerBuilder> beforeBuild)
        {
            autoMock = AutoMock.GetLoose(beforeBuild);
        }

        public TEntity Resolve<TEntity>()
        {
            return autoMock.Create<TEntity>();
        }

        public static string TestUniqueId => TestContext.Current?.Test?.UniqueID[..10] ?? DateTime.Now.Ticks.ToString()[..10];

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

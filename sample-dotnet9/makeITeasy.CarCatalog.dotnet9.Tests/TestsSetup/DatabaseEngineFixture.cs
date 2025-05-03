using makeITeasy.CarCatalog.dotnet9.Tests.TestsSetup;

using Xunit;

[assembly: AssemblyFixture(typeof(DatabaseEngineFixture))]

namespace makeITeasy.CarCatalog.dotnet9.Tests.TestsSetup
{
    public enum DatabaseType
    {
        Unknown,
        MsSql,
        SqlLite
    }

    public class DatabaseEngineFixture : IDisposable
    {
        public DatabaseType CurentDatabaseType { get; set; }

        public DatabaseEngineFixture()
        {
            CurentDatabaseType = DatabaseType.MsSql;
        }

        public void Dispose()
        {
        }
    }
}

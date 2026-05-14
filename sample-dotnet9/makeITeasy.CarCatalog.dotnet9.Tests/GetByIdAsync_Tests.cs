using System.Linq.Expressions;
using AwesomeAssertions;
using makeITeasy.CarCatalog.dotnet9.Core.Services.Interfaces;
using makeITeasy.CarCatalog.dotnet9.Models;
using makeITeasy.CarCatalog.dotnet9.Tests.Catalogs;
using makeITeasy.CarCatalog.dotnet9.Tests.TestsSetup;
using Xunit;

namespace makeITeasy.CarCatalog.dotnet9.Tests
{
    public class GetByIdAsync_Tests(DatabaseEngineFixture databaseEngineFixture) : UnitTestAutofacService(databaseEngineFixture)
    {
        [Fact]
        public async Task GetByIdAsync_SimpleKey_NoIncludes_Success()
        {
            ICarService carService = Resolve<ICarService>();
            string suffix = TimeOnly.FromDateTime(DateTime.Now).ToString("hhmmssfffffff");
            Car entity = CarsCatalog.CitroenC4(suffix);
            await carService.CreateAsync(entity);

            Car? result = await carService.GetByIdAsync(entity.Id);

            result.Should().NotBeNull();
            result!.Id.Should().Be(entity.Id);
            result.Name.Should().Be(entity.Name);
        }

        [Fact]
        public async Task GetByIdAsync_SimpleKey_WithIncludes_Success()
        {
            ICarService carService = Resolve<ICarService>();
            string suffix = TimeOnly.FromDateTime(DateTime.Now).ToString("hhmmssfffffff");
            Car entity = CarsCatalog.CitroenC4(suffix);
            await carService.CreateAsync(entity);

            Car? resultNoInclude = await carService.GetByIdAsync(entity.Id);
            resultNoInclude.Should().NotBeNull();
            resultNoInclude!.BrandId.Should().BeGreaterThan(0);
            resultNoInclude!.Brand.Should().BeNull();

            Car? resultWithInclude = await carService.GetByIdAsync(entity.Id, [x => x.Brand]);
            resultWithInclude.Should().NotBeNull();
            resultWithInclude!.Brand.Should().NotBeNull();
            resultWithInclude.Brand.Name.Should().StartWith("Citroen");
        }

        [Fact]
        public async Task GetByIdAsync_CompositeKey_NoIncludes_Success()
        {
            ICompositeKeyTableService compositeKeyTableService = Resolve<ICompositeKeyTableService>();
            CompositeKeyTable entity = new()
            {
                Id1 = Random.Shared.Next(),
                Id2 = Random.Shared.Next(),
                Col1 = 123,
                CompositeKeySubTables =
                [
                    new CompositeKeySubTable() { Value = "Value1" },
                    new CompositeKeySubTable() { Value = "Value2" },
                ]
            };
            await compositeKeyTableService.CreateAsync(entity);

            CompositeKeyTable? result = await compositeKeyTableService.GetByIdAsync(entity.DatabaseID);

            result.Should().NotBeNull();
            result!.Id1.Should().Be(entity.Id1);
            result.Id2.Should().Be(entity.Id2);
        }

        [Fact]
        public async Task GetByIdAsync_CompositeKey_WithEmptyIncludes_ShouldWork()
        {
            ICompositeKeyTableService compositeKeyTableService = Resolve<ICompositeKeyTableService>();
            CompositeKeyTable entity = new()
            {
                Id1 = Random.Shared.Next(),
                Id2 = Random.Shared.Next(),
                Col1 = 456,
                CompositeKeySubTables =
                [
                    new CompositeKeySubTable() { Value = "Value1" },
                    new CompositeKeySubTable() { Value = "Value2" },
                ]
            };
            await compositeKeyTableService.CreateAsync(entity);

            CompositeKeyTable? result = await compositeKeyTableService.GetByIdAsync(entity.DatabaseID, new List<Expression<Func<CompositeKeyTable, object>>>() {x => x.CompositeKeySubTables });

            result.Should().NotBeNull();
            result!.Id1.Should().Be(entity.Id1);
            result.Id2.Should().Be(entity.Id2);

            result.CompositeKeySubTables.Should().NotBeNull();
            result.CompositeKeySubTables.Select(x => x.Value).Should().BeEquivalentTo("Value1", "Value2");
        }
    }
}

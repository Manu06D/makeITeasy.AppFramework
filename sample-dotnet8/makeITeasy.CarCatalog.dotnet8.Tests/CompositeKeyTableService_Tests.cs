using AwesomeAssertions;
using makeITeasy.AppFramework.Core.Commands;
using makeITeasy.CarCatalog.dotnet8.Core.Services.Interfaces;
using makeITeasy.CarCatalog.dotnet8.Infrastructure.Data;
using makeITeasy.CarCatalog.dotnet8.Models;
using Xunit;

namespace makeITeasy.CarCatalog.dotnet8.Tests
{
    public class CompositeKeyTableService_Tests : UnitTestAutofacService<ServiceRegistrationAutofacModule>
    {
        public CompositeKeyTableService_Tests()
        {
            var t = Resolve<CarCatalogContext>();
            t.Database.EnsureCreated();
        }

        [Fact]
        public async Task IsValid_InValidObjectTest()
        {
            ICompositeKeyTableService compositeKeyTableService = Resolve<ICompositeKeyTableService>();

            CompositeKeyTable initialRow = new()
            {
                Id1 = Random.Shared.Next(),
                Id2 = Random.Shared.Next(),
                Col1 = 3,
            };

            var createResult = await compositeKeyTableService.CreateAsync(initialRow);
            createResult.Result.Should().Be(CommandState.Success);

            var getResult = await compositeKeyTableService.GetByIdAsync(initialRow.DatabaseID);
            getResult.Should().BeEquivalentTo(initialRow);

            initialRow.Col1 = 4;
            var updateResult = await compositeKeyTableService.UpdateAsync(initialRow);
            updateResult.Result.Should().Be(CommandState.Success);

            getResult = await compositeKeyTableService.GetByIdAsync(initialRow.DatabaseID);
            getResult.Should().BeEquivalentTo(initialRow);
        }
    }
}

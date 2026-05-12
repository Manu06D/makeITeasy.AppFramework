using AwesomeAssertions;
using makeITeasy.AppFramework.Core.Commands;
using makeITeasy.CarCatalog.dotnet9.Core.Services.Interfaces;
using makeITeasy.CarCatalog.dotnet9.Models;
using makeITeasy.CarCatalog.dotnet9.Tests.TestsSetup;
using Xunit;

namespace makeITeasy.CarCatalog.dotnet9.Tests
{
    public class CompositeKeyTableService_Tests(DatabaseEngineFixture databaseEngineFixture) : UnitTestAutofacService(databaseEngineFixture)
    {
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

            CommandResult<CompositeKeyTable> createResult = await compositeKeyTableService.CreateAsync(initialRow);
            createResult.Result.Should().Be(CommandState.Success);

            CompositeKeyTable? getResult = await compositeKeyTableService.GetByIdAsync(initialRow.DatabaseID);
            getResult.Should().BeEquivalentTo(initialRow);

            initialRow.Col1 = 4;
            CommandResult<CompositeKeyTable> updateResult = await compositeKeyTableService.UpdateAsync(initialRow);
            updateResult.Result.Should().Be(CommandState.Success);

            getResult = await compositeKeyTableService.GetByIdAsync(initialRow.DatabaseID);
            getResult.Should().BeEquivalentTo(initialRow);
        }
    }
}

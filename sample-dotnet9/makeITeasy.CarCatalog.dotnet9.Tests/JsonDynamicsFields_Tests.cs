using FluentAssertions;

using makeITeasy.AppFramework.Core.Commands;
using makeITeasy.CarCatalog.dotnet9.Core.Services.Interfaces;
using makeITeasy.CarCatalog.dotnet9.Core.Services.Queries.CarQueries;
using makeITeasy.CarCatalog.dotnet9.Core.Services.Queries.EngineQueries;
using makeITeasy.CarCatalog.dotnet9.Models;
using makeITeasy.CarCatalog.dotnet9.Tests.TestsSetup;

using Xunit;

namespace makeITeasy.CarCatalog.dotnet9.Tests
{
    public class JsonDynamicsFields_Tests(DatabaseEngineFixture databaseEngineFixture) : UnitTestAutofacService(databaseEngineFixture)
    {
        [Fact]
        public async Task CreateAndGet_JsonDynamicFieldTest()
        {
            ICarService carService = Resolve<ICarService>();
            string suffix = TimeOnly.FromDateTime(DateTime.Now).ToString("hhmmssffff");

            Car newCar = new()
            {
                Name = "C3" + suffix,
                ReleaseYear = 2011,
                Brand = new()
                {
                    Name = "Citroen",
                    Country = new Country()
                    {
                        Name = "France",
                        CountryCode = "FR"
                    }
                },
                CarDetails =
                [
                    new() {
                        CarDetails = new Models.DynamicModels.CarDetailsModel()
                        {
                            Name = "XXX",
                            ImageUrl="http://foo.bar"
                        }
                    }
                ]
            };

            var result = await carService.CreateAsync(newCar);

            var getResult = await carService.GetFirstByQueryAsync(new BasicCarQuery()
            {
                ID = result?.Entity?.Id,
                Includes = new List<System.Linq.Expressions.Expression<Func<Car, object>>>(){
                {
                    x => x.CarDetails
                }
            }
            });

            getResult.CarDetails.Should().NotBeNullOrEmpty();
            getResult.CarDetails.First().CarDetails.Name.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task CreateAndGet_EF9JsonFieldTest()
        {
            IEngineService engineService = Resolve<IEngineService>();
            string suffix = TimeOnly.FromDateTime(DateTime.Now).ToString("hhmmssffff");

            const bool hasTurbo = true;
            const int powerHorses = 120;

            Engine engine = new()
            {
                Name = "16v 1.2 Puretech" + suffix,
                Details = new EngineDetails()
                {
                    HasTurbo = hasTurbo,
                    PowerHorse = powerHorses,
                    Characteristics =
                    [
                        new Characteristic() { Name = "Energy", Value = "Petrol"}
                    ]
                }
            };

            CommandResult<Engine> result = await engineService.CreateAsync(engine);

            Engine getResult = await engineService.GetFirstByQueryAsync(new BaseEngineQuery() { ID = result?.Entity?.Id });
            getResult.Details.HasTurbo.Should().Be(hasTurbo);
            getResult.Details.PowerHorse.Should().Be(powerHorses);
            getResult.Details.Characteristics.Should().NotBeEmpty();
        }

        [Fact]
        public async Task Query_EF9JsonFieldTest()
        {
            IEngineService engineService = Resolve<IEngineService>();
            string suffix = TimeOnly.FromDateTime(DateTime.Now).ToString("hhmmssffff");

            Engine engine = new()
            {
                Name = "16v 1.2 Puretech" + suffix,
                Details = new EngineDetails()
                {
                    HasTurbo = true,
                    PowerHorse = 120,
                    Characteristics =
                    [
                        new () { Name = "Energy", Value = "Petrol"}
                    ]
                }
            };

            _ = await engineService.CreateAsync(engine);

            Engine engine2 = new()
            {
                Name = "1.3 HDI" + suffix,
                Details = new EngineDetails()
                {
                    HasTurbo = true,
                    PowerHorse = 90,
                    Characteristics =
                    [
                        new () { Name = "Energy", Value = "Diesel"}
                    ]
                }
            };

            _ = await engineService.CreateAsync(engine2);

            IList<Engine> getResult = (await engineService.QueryAsync(new BaseEngineQuery() { MinimalHorspower = 120, NameSuffix = suffix })).Results;

            getResult.Should().HaveCount(1);
            getResult[0].Name.Should().StartWith("16v 1.2 Puretech");

            getResult = (await engineService.QueryAsync(new BaseEngineQuery() { Characteristic = new Tuple<string, string>("Energy", "Diesel") })).Results;

            getResult.Should().NotBeEmpty();
            getResult[0].Name.Should().StartWith("1.3 HDI");
        }
    }
}

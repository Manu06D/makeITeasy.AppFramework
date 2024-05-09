using FluentAssertions;

using makeITeasy.AppFramework.Core.Commands;
using makeITeasy.CarCatalog.dotnet8.Core.Services.Interfaces;
using makeITeasy.CarCatalog.dotnet8.Core.Services.Queries.CarQueries;
using makeITeasy.CarCatalog.dotnet8.Core.Services.Queries.EngineQueries;
using makeITeasy.CarCatalog.dotnet8.Infrastructure.Data;
using makeITeasy.CarCatalog.dotnet8.Models;

using Xunit;

namespace makeITeasy.CarCatalog.dotnet8.Tests
{
    public class JsonDynamicsFields_Tests : UnitTestAutofacService<ServiceRegistrationAutofacModule>
    {
        private ICarService carService;
        private IEngineService engineService;

        public JsonDynamicsFields_Tests()
        {
            carService = Resolve<ICarService>();
            engineService = Resolve<IEngineService>();
            var t = Resolve<CarCatalogContext>();
            t.Database.EnsureCreated();
        }

        ~JsonDynamicsFields_Tests()
        {
            carService = null;
        }

        [Fact]
        public async Task CreateAndGet_JsonDynamicFieldTest()
        {
            string usageKey = "usage";
            string usageValue = "city";

            Car newCar = new()
            {
                Name = "C3",
                ReleaseYear = 2011,
                Brand = new ()
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
                            Details = new Dictionary<string, string>() { { usageKey, usageValue  } }
                        }
                    }
                ]
            };

            var result = await carService.CreateAsync(newCar);

            var getResult = await carService.GetFirstByQueryAsync(new BaseCarQuery()
            {
                ID = result?.Entity?.Id,
                Includes = new List<System.Linq.Expressions.Expression<Func<Car, object>>>(){
                {
                    x => x.CarDetails
                }
            }});

            getResult.CarDetails.Should().NotBeNullOrEmpty();
            getResult.CarDetails.First().CarDetails.Details.Should().NotBeNullOrEmpty();
            getResult.CarDetails.First().CarDetails.Details[usageKey].Should().Be(usageValue);
        }

        [Fact]
        public async Task CreateAndGet_EF8JsonFieldTest()
        {
            bool hasTurbo = true;
            int powerHorses = 120;

            Engine engine = new ()
            {
                Name="16v 1.2 Puretech",
                Details = new EngineDetails()
                {
                    HasTurbo = hasTurbo,
                    PowerHorse = powerHorses
                }
            };

            CommandResult<Engine> result = await engineService.CreateAsync(engine);

            Engine getResult = await engineService.GetFirstByQueryAsync(new BaseEngineQuery() { ID = result?.Entity?.Id});
            getResult.Details.HasTurbo.Should().Be(hasTurbo);
            getResult.Details.PowerHorse.Should().Be(powerHorses);
        }
    }
}

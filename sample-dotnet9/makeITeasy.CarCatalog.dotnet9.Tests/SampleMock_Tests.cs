using Autofac;
using Autofac.Extras.Moq;

using FluentAssertions;

using makeITeasy.CarCatalog.dotnet9.Core.Ports;
using makeITeasy.CarCatalog.dotnet9.Core.Services;
using makeITeasy.CarCatalog.dotnet9.Core.Services.Interfaces;
using makeITeasy.CarCatalog.dotnet9.Infrastructure.Data;
using makeITeasy.CarCatalog.dotnet9.Models.Custom;

using Microsoft.Extensions.Logging;

using Moq;

using Xunit;

namespace makeITeasy.CarCatalog.dotnet9.Tests
{
    public class SampleMock_Tests
    {
        [Fact]
        public async Task SampleTest_WithMock()
        {
            const string brandName = "Ferrari";
            const int carCount = 3;

            Mock<ILogger<CarService>> loggerMock = new();

            Mock<ICarRepository> carRepositoryMock = new() { };
            carRepositoryMock.Setup(x => x.GroupByBrandAndCountAsync()).ReturnsAsync([new () { BrandName = brandName, CarCount = carCount }]);

            using (AutoMock autoMock = AutoMock.GetLoose(cfg =>
            {
                cfg.RegisterModule<ServiceRegistrationAutofacModule>();
                cfg.RegisterMock(loggerMock);
                cfg.RegisterMock(carRepositoryMock);
            }))
            {
                CarCatalogContext dbContext = autoMock.Create<CarCatalogContext>();
                dbContext.Database.EnsureCreated();
                ICarService carService = autoMock.Create<ICarService>();

                List<BrandGroupByCarCount> brands = await carService.GetBrandWithCountAsync();

                loggerMock.Verify(
                    x => x.Log<It.IsAnyType>(
                        LogLevel.Information,
                        It.IsAny<EventId>(),
                        It.Is<It.IsAnyType>((message, _) => message.ToString().StartsWith("inside GetBrandWithCountAsync method")),
                        It.IsAny<Exception>(),
                        (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()),
                        Times.Once);

                brands.Should().HaveCount(1);
                brands[0].BrandName.Should().Be(brandName);
                brands[0].CarCount.Should().Be(carCount);
            }
        }
    }
}

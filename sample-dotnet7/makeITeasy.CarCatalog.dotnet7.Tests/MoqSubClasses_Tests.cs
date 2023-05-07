using Autofac;
using Autofac.Extras.Moq;

using FluentAssertions;

using makeITeasy.AppFramework.Core.Interfaces;
using makeITeasy.CarCatalog.dotnet7.Core.Services;
using makeITeasy.CarCatalog.dotnet7.Core.Services.Interfaces;
using makeITeasy.CarCatalog.dotnet7.Models;

using Moq;

using System.Threading.Tasks;

using Xunit;

namespace makeITeasy.CarCatalog.dotnet7.Tests
{
    public class MoqSubClasses_Tests
    {
        [Fact]
        public async Task XX()
        {
            const string brandName = "MyBrand";

            Mock<IAsyncRepository<Brand>> repositoryMock = new() { };
            repositoryMock.Setup(x => x.AddAsync(It.IsAny<Brand>(), true)).ReturnsAsync(new Brand() { Name = brandName });
            repositoryMock.Setup(x => x.DeleteAsync(It.IsAny<Brand>(), true)).CallBase();

            using (AutoMock autoMock = AutoMock.GetLoose(cfg =>
            {
                cfg.RegisterModule<ServiceRegistrationAutofacModule>();
                cfg.RegisterMock(repositoryMock);
            }))
            {
                //HACK : IValidator is not working with AutoMock
                IBrandService brandService = new BrandService(autoMock.Create<IAsyncRepository<Brand>>(), null);

                var xx =  await brandService.CreateAsync(new Brand() { Name = "XXX"});

                await brandService.DeleteAsync(new Brand());

                xx.Entity.Name.Should().Be(brandName);
            }
        }
    }
}

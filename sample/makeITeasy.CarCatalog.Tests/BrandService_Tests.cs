using AutoMapper;
using makeITeasy.AppFramework.Core.Interfaces;
using makeITeasy.CarCatalog.Core.Domains.BrandDomain;
using makeITeasy.CarCatalog.Core.Domains.CarDomain;
using makeITeasy.CarCatalog.Infrastructure.Data;
using makeITeasy.CarCatalog.Models;
using makeITeasy.CarCatalog.Tests.Catalogs;
using Xunit;
using makeITeasy.CarCatalog.Models.Collections;
using makeITeasy.CarCatalog.Core.Domains.BrandDomain.Queries;
using System.Threading.Tasks;
using FluentAssertions;
using System.Collections.Generic;
using System.Linq;

namespace makeITeasy.CarCatalog.Tests
{
    public class BrandService_Tests : UnitTestAutofacService<ServiceRegistrationAutofacModule>
    {
        private IBrandService brandService;
        private ICarService carService;

        public BrandService_Tests()
        {
            brandService = Resolve<IBrandService>();
            carService = Resolve<ICarService>();

            var t = Resolve<CarCatalogContext>();
            t.Database.EnsureCreated();
        }

        ~BrandService_Tests()
        {
            brandService = null;
            carService = null;
        }

        public class BrandInfo : IMapFrom<Brand>
        {
            public string Name { get; set; }
            public int FirstRelease { get; set; }
            public int NbCars { get; set; }

            public void Mapping(Profile profile)
            {
                if (profile != null)
                {
                    profile.CreateMap<Brand, BrandInfo>()
                        .ForMember(dest => dest.NbCars, src => src.MapFrom(x => x.Cars.Count))
                        .ForMember(dest => dest.FirstRelease, src => src.MapFrom(x => x.Cars.MinimalReleaseYear()));
                }
            }
        }

        [Fact]
        public async Task CreateAndGet_ListWithFunctionTest()
        {
            var newCars = TestCarsCatalog.GetValidCars(50);

            newCars.ForEach(async x => await carService.Create(x));

            var getResult = await brandService.QueryWithProjectionAsync<BrandInfo>(new BaseBrandQuery());

            getResult.Results.Should().OnlyContain(x => x.FirstRelease > 0 && x.NbCars > 0);
        }

        public class SmallBrandInfo : IMapFrom<Brand>
        {
            public string Name { get; set; }
            public List<SmallCarInfo> Cars { get; set; }
        }

        public class SmallCarInfo : IMapFrom<Car>
        {
            public string Name { get; set; }
        }

        [Fact]
        public async Task CreateAndGet_ListWith2LevelMappingTest()
        {
            var newCars = TestCarsCatalog.GetValidCars(50);

            newCars.ForEach(async x => await carService.Create(x));

            var getResult = await brandService.QueryWithProjectionAsync<SmallBrandInfo>(new BaseBrandQuery());

            getResult.Results.Should().OnlyContain(x => x.Name != null && x.Cars != null && x.Cars.Select(x => x.Name).Count() >= 1 );

        }
    }
}

using AutoMapper;
using makeITeasy.AppFramework.Core.Interfaces;
using makeITeasy.CarCatalog.Infrastructure.Data;
using makeITeasy.CarCatalog.Models;
using makeITeasy.CarCatalog.Tests.Catalogs;
using Xunit;
using makeITeasy.CarCatalog.Models.Collections;
using System.Threading.Tasks;
using FluentAssertions;
using System.Collections.Generic;
using System.Linq;
using makeITeasy.CarCatalog.Core.Services.Interfaces;
using makeITeasy.CarCatalog.Core.Services.Queries.BrandQueries;
using makeITeasy.AppFramework.Core.Models.Exceptions;
using System.Transactions;
using makeITeasy.CarCatalog.Core.Services.Queries.CarQueries;
using makeITeasy.AppFramework.Models;

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
            var newCars = TestCarsCatalog.GetCars();

            newCars.ForEach(async x => await carService.CreateAsync(x));

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
            var newCars = TestCarsCatalog.GetCars();

            newCars.ForEach(async x => await carService.CreateAsync(x));

            var getResult = await brandService.QueryWithProjectionAsync<SmallBrandInfo>(new BaseBrandQuery());

            getResult.Results.Should().OnlyContain(x => x.Name != null && x.Cars != null && x.Cars.Select(x => x.Name).Count() >= 1);

        }

        [Fact]
        public void MissingValidator_Test()
        {
            var newBrand = new Brand
            {
                Name = "x"
            };

            brandService.Invoking(y => y.IsValid(newBrand)).Should().Throw<ValidatorNotFoundException>();
        }


        [Fact]
        public async Task Transaction_Tests()
        {
            var newBrand = new Brand
            {
                Name = "x",
                Country = new Country()
                {
                    Name = "France",
                    CountryCode = "FR"
                }
            };

            var newBrand2 = new Brand
            {
                Name = "xx",
            };

            using (var scope = new TransactionScope(TransactionScopeOption.Required))
            {
                try
                {
                    _ = await brandService.CreateAsync(newBrand);

                    var localSearch = await brandService.QueryAsync(new BaseBrandQuery() { });
                    localSearch.Results.Should().HaveCount(1);

                    _ = await brandService.CreateAsync(newBrand2);

                    scope.Complete();

                    newBrand.Id.Should().BePositive();
                    newBrand2.Id.Should().BePositive();

                }
                catch
                {
                    scope.Dispose();
                }
            }

            var search = await brandService.QueryAsync(new BaseBrandQuery() { });
            search.Results.Should().HaveCount(0);
        }

        [Fact]
        public async Task TestFluent()
        {
            //ICanAddFunctionFilter canAddFunctionFilter =
            //    FluentQueryBuilder.Create<Car>(new BaseCarQuery());

            QueryBuilder.Create(new BaseCarQuery()).Where(x => x.ReleaseYear == 2010);

            //new BaseCarQuery().Where();
        }
    }
}

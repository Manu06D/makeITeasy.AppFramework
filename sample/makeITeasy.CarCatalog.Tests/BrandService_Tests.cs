﻿using AutoMapper;
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
using System;

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

            brandService.Invoking(y => y.Validate(newBrand)).Should().Throw<ValidatorNotFoundException>();
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

            //Dont' work in unit test due to leak of support of transaction of sqllite
            using (var scope = new TransactionScope(TransactionScopeOption.Required, TransactionScopeAsyncFlowOption.Enabled))
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
        public async Task EF5IncludeWithFunction_Test()
        {
            TestCarsCatalog.SaveCarsInDB(carService);

            var getResult = await brandService.QueryAsync(
                new BaseBrandQuery()
                {
                    Includes = new List<System.Linq.Expressions.Expression<Func<Brand, object>>>() { x => x.Cars.Where(x => x.Name.StartsWith("A3")) },
                }
                , includeCount: true);

            getResult.Results.Where(x => x.Cars.Count >= 1).Should().HaveCount(1);
            getResult.Results.First(x => x.Cars.Count >= 1).Cars.FirstOrDefault().Name.Should().Be("A3");
        }
    }
}

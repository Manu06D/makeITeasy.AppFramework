using AutoMapper;
using makeITeasy.AppFramework.Core.Interfaces;
using makeITeasy.CarCatalog.dotnet9.Models;
using Xunit;
using makeITeasy.CarCatalog.dotnet9.Models.Collections;
using AwesomeAssertions;
using makeITeasy.CarCatalog.dotnet9.Core.Services.Interfaces;
using makeITeasy.CarCatalog.dotnet9.Core.Services.Queries.BrandQueries;
using makeITeasy.AppFramework.Core.Models.Exceptions;
using System.Transactions;
using makeITeasy.CarCatalog.dotnet9.Tests.Catalogs;
using makeITeasy.CarCatalog.dotnet9.Tests.TestsSetup;
using makeITeasy.AppFramework.Core.Commands;
using makeITeasy.AppFramework.Models;
using Microsoft.EntityFrameworkCore;

namespace makeITeasy.CarCatalog.dotnet9.Tests
{
    public class BrandService_Tests(DatabaseEngineFixture databaseEngineFixture) : UnitTestAutofacService(databaseEngineFixture)
    {
        public class BrandInfo : IMapFrom<Brand>
        {
            public string? Name { get; set; }
            public int FirstRelease { get; set; }
            public int CarsCount { get; set; }

            public void Mapping(Profile profile)
            {
                profile?.CreateMap<Brand, BrandInfo>()
                        .ForMember(dest => dest.CarsCount, src => src.MapFrom(x => x.Cars.Count))
                        .ForMember(dest => dest.FirstRelease, src => src.MapFrom(x => x.Cars.MinimalReleaseYear()));
            }
        }

        [Fact]
        public async Task CreateAndGet_ListWithFunctionTest()
        {
            (_, IBrandService brandService, Brand citroenBrand, string suffix, _) = await CreateCarsAsync();

            var getResult = await brandService.QueryWithProjectionAsync<BrandInfo>(new BasicBrandQuery());

            getResult.Results.Where(x => x.Name == citroenBrand.Name).Should().HaveCount(1);
            getResult.Results.First(x => x.Name == citroenBrand.Name).CarsCount.Should().Be(2);
            getResult.Results.First(x => x.Name == citroenBrand.Name).FirstRelease.Should()
                .Be(new List<Car>() { CarsCatalog.CitroenC4(), CarsCatalog.CitroenC5() }.Min(x => x.ReleaseYear));
        }

        public class SmallBrandInfo : IMapFrom<Brand>
        {
            public string? Name { get; set; }
            public List<SmallCarInfo>? Cars { get; set; }
        }

        public class SmallCarInfo : IMapFrom<Car>
        {
            public string? Name { get; set; }
        }

        [Fact]
        public async Task CreateAndGet_ListWith2LevelMappingTest()
        {
            (_, IBrandService brandService, Brand citroenBrand, string suffix, _) = await CreateCarsAsync();

            var getResult = await brandService.QueryWithProjectionAsync<SmallBrandInfo>(new BasicBrandQuery());

            getResult.Results.Where(x => x.Name == citroenBrand.Name).Should().HaveCount(1);
            getResult.Results.First(x => x.Name == citroenBrand.Name).Name.Should().EndWith(suffix);
            getResult.Results.First(x => x.Name == citroenBrand.Name).Cars.Should().HaveCount(2);
            getResult.Results.First(x => x.Name == citroenBrand.Name).Cars.Should().OnlyContain(x => x.Name != null);
        }

        [Fact]
        public void MissingValidator_Test()
        {
            ICountryService countryService = Resolve<ICountryService>();

            var newCountry = new Country
            {
                Name = "x"
            };

            countryService.Invoking(y => y.Validate(newCountry)).Should().Throw<ValidatorNotFoundException>();
        }

        [Fact]
        public async Task TransactionWithError_Tests()
        {
            //Dont' work in unit test due to leak of support of transaction of sqllite
            if (DatabaseEngineFixture.CurentDatabaseType == DatabaseType.MsSql)
            {
                IBrandService brandService = Resolve<IBrandService>();
                string suffix = TestHelper.UniqueSuffix();

                var newBrand = new Brand
                {
                    Name = "x" + suffix,
                    Country = new Country()
                    {
                        Name = "France",
                        CountryCode = "FR"
                    }
                };

                var newBrand2 = new Brand
                {
                    Name = "xx" + suffix,
                };

                using (TransactionScope scope = new(
                    TransactionScopeOption.Required,
                    new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted },
                    TransactionScopeAsyncFlowOption.Enabled))
                {
                    try
                    {
                        _ = await brandService.CreateAsync(newBrand);

                        var localSearch = await brandService.QueryAsync(new BasicBrandQuery() { NameSuffix = suffix });
                        localSearch.Results.Where(x => x.Name.EndsWith(suffix)).Should().HaveCount(1);

                        var dbCreationResult = await brandService.CreateAsync(newBrand2);

                        if (dbCreationResult.Result == CommandState.Error)
                        {
                            throw new Exception($"Error {dbCreationResult.Message}");
                        }

                        scope.Complete();

                        newBrand.Id.Should().BePositive();
                        newBrand2.Id.Should().BePositive();
                    }
                    catch (Exception)
                    {
                        scope.Dispose();
                    }
                }

                var search = await brandService.QueryAsync(new BasicBrandQuery() { NameSuffix = suffix });
                search.Results.Should().HaveCount(0);
            }
        }

        [Fact]
        public async Task EFCoreIncludeWithFunction_Test()
        {
            (_, IBrandService brandService, Brand citroenBrand, string suffix, _) = await CreateCarsAsync();
            (_, _, Brand citroenBrand2, string suffix2, _) = await CreateCarsAsync();

            var getResult = await brandService.QueryAsync(
                new BasicBrandQuery()
                {
                    Includes = [x => x.Cars.Where(x => x.Name.EndsWith(suffix))],
                }
                , includeCount: true);

            getResult.Results.Where(x => x.Name.EndsWith(suffix)).Should().HaveCount(1);
            getResult.Results.Where(x => x.Name.EndsWith(suffix2)).Should().HaveCount(1);
            getResult.Results.SelectMany(x => x.Cars).Should().HaveCount(2);
            getResult.Results.SelectMany(x => x.Cars).Should().AllSatisfy(x => x.Name.Should().EndWith(suffix));
        }

        public class CustomBrand : IMapFrom<Brand>
        {
            public int Id { get; set; }
            public string? Name { get; set; }
        }

        [Fact]
        public async Task EFCoreIncludeWithFunctionAndProjection_Test()
        {
            (_, IBrandService brandService, Brand citroenBrand, string suffix, _) = await CreateCarsAsync();

            var getResult = await brandService.QueryWithProjectionAsync<CustomBrand>(
                new BasicBrandQuery()
                {
                    Includes =
                        [
                            x => x.Cars.Where(x => x.Name.EndsWith(suffix)),
                        ]
                }
                , includeCount: true);

            getResult.Results.Where(x => x.Name?.EndsWith(suffix) == true).Should().HaveCount(1);
        }

        [Fact]
        public async Task SplitQuery_ViaSpecification_ShouldThrowNotSupportedException()
        {
            (_, IBrandService brandService, _, _, _) = await CreateCarsAsync();

            var query = new BasicBrandQuery { Includes = [x => x.Cars], SplitQuery = true };

            Func<Task> act = async () => await brandService.QueryAsync(query);

            await act.Should().ThrowAsync<NotSupportedException>();
        }

        [Fact]
        public async Task SplitQuery_ViaQueryBuilder_ShouldThrowNotSupportedException()
        {
            (_, IBrandService brandService, _, _, _) = await CreateCarsAsync();

            var query = QueryBuilder.Create<BasicBrandQuery, Brand>()
                .Include(x => x.Cars)
                .WithSplitQuery()
                .Build();

            Func<Task> act = async () => await brandService.QueryAsync(query);

            await act.Should().ThrowAsync<NotSupportedException>();
        }

        [Fact]
        public void SplitQuery_SpecificationEvaluator_ShouldThrowNotSupportedException()
        {
            var spec = new BasicBrandQuery { Includes = [x => x.Cars], SplitQuery = true };

            Action act = () => AppFramework.Infrastructure.EntityFramework.Persistence.SpecificationEvaluator<Brand>.GetQuery(new List<Brand>().AsQueryable(), spec);

            act.Should().Throw<NotSupportedException>();
        }

        [Fact]
        public async Task SplitQuery_False_ShouldNotThrow()
        {
            (_, IBrandService brandService, _, _, _) = await CreateCarsAsync();

            var query = new BasicBrandQuery { Includes = [x => x.Cars], SplitQuery = false };

            var result = await brandService.QueryAsync(query);

            result.Results.Should().NotBeNull();
        }

        [Fact]
        public async Task TagWith_ViaSpecification_SuccessTest()
        {
            (_, IBrandService brandService, _, _, _) = await CreateCarsAsync();

            var query = new BasicBrandQuery { TagWith = "BrandQueryTag" };
            var result = await brandService.QueryAsync(query);
            result.Results.Should().NotBeNull();
        }

        [Fact]
        public async Task TagWith_ViaQueryBuilder_SuccessTest()
        {
            (_, IBrandService brandService, _, _, _) = await CreateCarsAsync();

            var query = QueryBuilder.Create<BasicBrandQuery, Brand>()
                .TagWith("QueryBuilderTag")
                .Build();

            query.TagWith.Should().Be("QueryBuilderTag");

            var result = await brandService.QueryAsync(query);
            result.Results.Should().NotBeNull();
        }

        [Fact]
        public void TagWith_SpecificationEvaluator_GeneratesSqlComment_Test()
        {
            var dbFactory = Resolve<IDbContextFactory<Infrastructure.Data.CarCatalogContext>>();
            using var dbContext = dbFactory.CreateDbContext();

            var spec = new BasicBrandQuery { TagWith = "MySpecialBrandTag" };
            var query = AppFramework.Infrastructure.EntityFramework.Persistence.SpecificationEvaluator<Brand>.GetQuery(dbContext.Set<Brand>().AsQueryable(), spec);

            string? sql = query.ToQueryString();
            sql.Should().Contain("-- MySpecialBrandTag");
        }
    }
}

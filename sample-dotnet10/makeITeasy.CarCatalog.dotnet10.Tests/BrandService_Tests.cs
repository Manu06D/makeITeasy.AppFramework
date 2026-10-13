using AutoMapper;

using AwesomeAssertions;

using makeITeasy.AppFramework.Core.Commands;
using makeITeasy.AppFramework.Core.Interfaces;
using makeITeasy.AppFramework.Core.Models.Exceptions;
using makeITeasy.AppFramework.Core.Queries;
using makeITeasy.CarCatalog.dotnet10.Core.Services.Interfaces;
using makeITeasy.CarCatalog.dotnet10.Core.Services.Queries.BrandQueries;
using makeITeasy.CarCatalog.dotnet10.Models;
using makeITeasy.CarCatalog.dotnet10.Models.Collections;
using makeITeasy.CarCatalog.dotnet10.Tests.Catalogs;
using makeITeasy.CarCatalog.dotnet10.Tests.TestsSetup;

using System.Transactions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using makeITeasy.AppFramework.Models;

using Xunit;

namespace makeITeasy.CarCatalog.dotnet10.Tests
{
    public class BrandService_Tests(DatabaseFixture databaseEngineFixture) : AutofacFixture(databaseEngineFixture)
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
            (_, IBrandService brandService, Brand citroenBrand, string suffix, _) = await CarsCatalog.CreateCarsAsync(this);

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
            (_, IBrandService brandService, Brand citroenBrand, string suffix, _) = await CarsCatalog.CreateCarsAsync(this);


            QueryResult<SmallBrandInfo> getResult = await brandService.QueryWithProjectionAsync<SmallBrandInfo>(new BasicBrandQuery());

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
            if (GlobalTestSetup.DatabaseType == DatabaseType.MsSql)
            {
                IBrandService brandService = Resolve<IBrandService>();

                var newBrand = new Brand
                {
                    Name = "x" + TestUniqueId,
                    Country = new Country()
                    {
                        Name = "France",
                        CountryCode = "FR"
                    }
                };

                var newBrand2 = new Brand
                {
                    Name = "xx" + TestUniqueId,
                };

                using (TransactionScope scope = new(
                    TransactionScopeOption.Required,
                    new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted },
                    TransactionScopeAsyncFlowOption.Enabled))
                {
                    try
                    {
                        _ = await brandService.CreateAsync(newBrand);

                        var localSearch = await brandService.QueryAsync(new BasicBrandQuery() { NameSuffix = TestUniqueId });
                        localSearch.Results.Where(x => x.Name.EndsWith(TestUniqueId)).Should().HaveCount(1);

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

                var search = await brandService.QueryAsync(new BasicBrandQuery() { NameSuffix = TestUniqueId });
                search.Results.Should().HaveCount(0);
            }
        }

        [Fact]
        public async Task EFCoreIncludeWithFunction_Test()
        {
            (_, IBrandService brandService, Brand citroenBrand, string suffix, _) = await CarsCatalog.CreateCarsAsync(this);
            (_, _, Brand citroenBrand2, string suffix2, _) = await CarsCatalog.CreateCarsAsync(this);

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
            (_, IBrandService brandService, Brand citroenBrand, string suffix, _) = await CarsCatalog.CreateCarsAsync(this);

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
        public void SplitQuery_SpecificationEvaluator_GeneratesSplitQuery_Test()
        {
            var connection = new Microsoft.Data.Sqlite.SqliteConnection("DataSource=:memory:");
            connection.Open();

            var options = new DbContextOptionsBuilder<Infrastructure.Data.CarCatalogContext>()
                .UseSqlite(connection)
                .Options;

            using var dbContext = new Infrastructure.Data.CarCatalogContext(options);

            var specWithSplit = new BasicBrandQuery { Includes = [x => x.Cars], SplitQuery = true };
            var specWithSingle = new BasicBrandQuery { Includes = [x => x.Cars], SplitQuery = false };

            var splitQuery = AppFramework.Infrastructure.EntityFramework.Persistence.SpecificationEvaluator<Brand>.GetQuery(dbContext.Set<Brand>().AsQueryable(), specWithSplit);
            var singleQuery = AppFramework.Infrastructure.EntityFramework.Persistence.SpecificationEvaluator<Brand>.GetQuery(dbContext.Set<Brand>().AsQueryable(), specWithSingle);

            string? splitSql = splitQuery.ToQueryString();
            string? singleSql = singleQuery.ToQueryString();

            splitSql.Should().NotBe(singleSql);
            singleSql.Should().Contain("JOIN");
            splitSql.Should().NotContain("JOIN");
        }

        [Fact]
        public async Task SplitQuery_ViaSpecification_SuccessTest()
        {
            (_, IBrandService brandService, _, _, _) = await CarsCatalog.CreateCarsAsync(this);

            var result = await brandService.QueryAsync(new BasicBrandQuery { Includes = [x => x.Cars], SplitQuery = true });

            result.Results.Should().NotBeEmpty();
            result.Results.Should().Contain(x => x.Cars.Count > 0);
        }

        [Fact]
        public async Task SplitQuery_ViaQueryBuilder_SuccessTest()
        {
            (_, IBrandService brandService, _, _, _) = await CarsCatalog.CreateCarsAsync(this);

            var query = QueryBuilder.Create<BasicBrandQuery, Brand>()
                .Include(x => x.Cars)
                .WithSplitQuery()
                .Build();

            query.SplitQuery.Should().BeTrue();

            var result = await brandService.QueryAsync(query);

            result.Results.Should().NotBeEmpty();
            result.Results.Should().Contain(x => x.Cars.Count > 0);
        }

        [Fact]
        public void SplitQuery_DbContextGlobalConfiguration_DefaultAndOverride_Test()
        {
            var connection = new Microsoft.Data.Sqlite.SqliteConnection("DataSource=:memory:");
            connection.Open();

            var options = new DbContextOptionsBuilder<Infrastructure.Data.CarCatalogContext>()
                .UseSqlite(connection, o => o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery))
                .Options;

            using var dbContext = new Infrastructure.Data.CarCatalogContext(options);

            var defaultSpec = new BasicBrandQuery { Includes = [x => x.Cars] };
            var overrideToSingleSpec = new BasicBrandQuery { Includes = [x => x.Cars], SplitQuery = false };
            var explicitSplitSpec = new BasicBrandQuery { Includes = [x => x.Cars], SplitQuery = true };

            var defaultQuery = AppFramework.Infrastructure.EntityFramework.Persistence.SpecificationEvaluator<Brand>.GetQuery(dbContext.Set<Brand>().AsQueryable(), defaultSpec);
            var singleQuery = AppFramework.Infrastructure.EntityFramework.Persistence.SpecificationEvaluator<Brand>.GetQuery(dbContext.Set<Brand>().AsQueryable(), overrideToSingleSpec);
            var splitQuery = AppFramework.Infrastructure.EntityFramework.Persistence.SpecificationEvaluator<Brand>.GetQuery(dbContext.Set<Brand>().AsQueryable(), explicitSplitSpec);

            string? defaultSql = defaultQuery.ToQueryString();
            string? singleSql = singleQuery.ToQueryString();
            string? splitSql = splitQuery.ToQueryString();

            defaultSql.Should().NotContain("JOIN");
            singleSql.Should().Contain("JOIN");
            splitSql.Should().NotContain("JOIN");
            splitSql.Should().Be(defaultSql);
        }

        [Fact]
        public async Task SplitQuery_DbContextGlobalConfiguration_ExecutionSuccessTest()
        {
            var connection = new Microsoft.Data.Sqlite.SqliteConnection("DataSource=:memory:");
            connection.Open();

            var services = new ServiceCollection();
            services.AddDbContextFactory<Infrastructure.Data.CarCatalogContext>(opt =>
            {
                opt.UseSqlite(connection, o => o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery));
            });
            var sp = services.BuildServiceProvider();
            var dbFactory = sp.GetRequiredService<IDbContextFactory<Infrastructure.Data.CarCatalogContext>>();

            using (var initContext = dbFactory.CreateDbContext())
            {
                initContext.Database.EnsureCreated();
                var country = new Country { Name = "France", CountryCode = "FR" };
                initContext.Countries.Add(country);
                initContext.Brands.Add(new Brand { Name = "TestBrand", Country = country, Cars = new List<Car> { new Car { Name = "Car1" } } });
                initContext.SaveChanges();
            }

            var brandRepo = new Infrastructure.Persistence.TransactionCarCatalogRepository<Brand>(dbFactory, Resolve<AutoMapper.IMapper>());
            using var customBrandService = new Core.Services.BrandService { EntityRepository = brandRepo };

            var defaultResult = await customBrandService.QueryAsync(new BasicBrandQuery { Includes = [x => x.Cars] });
            defaultResult.Results.Should().NotBeEmpty();
            defaultResult.Results.Should().Contain(x => x.Cars.Count > 0);

            var singleResult = await customBrandService.QueryAsync(new BasicBrandQuery { Includes = [x => x.Cars], SplitQuery = false });
            singleResult.Results.Should().NotBeEmpty();
            singleResult.Results.Should().Contain(x => x.Cars.Count > 0);
        }

        [Fact]
        public async Task SplitQuery_GetFirstByQueryAsync_SuccessTest()
        {
            (_, IBrandService brandService, _, _, _) = await CarsCatalog.CreateCarsAsync(this);

            var firstBrand = await brandService.GetFirstByQueryAsync(new BasicBrandQuery { Includes = [x => x.Cars], SplitQuery = true });

            firstBrand.Should().NotBeNull();
            firstBrand.Cars.Should().NotBeEmpty();
        }

        [Fact]
        public async Task TagWith_ViaSpecification_SuccessTest()
        {
            (_, IBrandService brandService, _, _, _) = await CarsCatalog.CreateCarsAsync(this);

            var query = new BasicBrandQuery { TagWith = "BrandQueryTag" };
            var result = await brandService.QueryAsync(query);
            result.Results.Should().NotBeNull();
        }

        [Fact]
        public async Task TagWith_ViaQueryBuilder_SuccessTest()
        {
            (_, IBrandService brandService, _, _, _) = await CarsCatalog.CreateCarsAsync(this);

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

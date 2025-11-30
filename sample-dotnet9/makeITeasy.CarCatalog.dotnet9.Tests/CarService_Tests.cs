using System.Linq.Expressions;

using AwesomeAssertions;

using makeITeasy.AppFramework.Core.Commands;
using makeITeasy.AppFramework.Core.Interfaces;
using makeITeasy.AppFramework.Models;
using makeITeasy.CarCatalog.dotnet9.Core.Services.Interfaces;
using makeITeasy.CarCatalog.dotnet9.Core.Services.Queries.CarQueries;
using makeITeasy.CarCatalog.dotnet9.Tests.Catalogs;
using makeITeasy.CarCatalog.dotnet9.Infrastructure.Data;
using makeITeasy.CarCatalog.dotnet9.Models;

using Microsoft.EntityFrameworkCore;

using Xunit;
using makeITeasy.AppFramework.Core.Queries;
using makeITeasy.CarCatalog.dotnet9.Tests.TestsSetup;

namespace makeITeasy.CarCatalog.dotnet9.Tests
{
    public class CarService_Tests(DatabaseEngineFixture databaseEngineFixture) : UnitTestAutofacService(databaseEngineFixture)
    {
        [Fact]
        public void IsValid_InValidObjectTest()
        {
            ICarService carService = Resolve<ICarService>();

            var newCar = new Car
            {
                Name = "x"
            };

            carService.Validate(newCar).Should().BeFalse();
        }

        [Fact]
        public void IsValid_ValidObjectTest()
        {
            ICarService carService = Resolve<ICarService>();
            var t = Resolve<CarCatalogContext>();

            t.Database.EnsureCreated();

            var newCar = new Car
            {
                Name = "xxxx"
            };

            carService.Validate(newCar).Should().BeTrue();
        }

        [Fact]
        public async Task Create_InValidObjectTest()
        {
            ICarService carService = Resolve<ICarService>();

            var newCar = new Car
            {
                Name = "x"
            };

            var result = await carService.CreateAsync(newCar);

            result.Result.Should().Be(CommandState.Error);
        }

        [Fact]
        public void Create_UncompleteObjectTest()
        {
            ICarService carService = Resolve<ICarService>();

            var newCar = new Car
            {
                Name = "xxx"
            };

            carService.Invoking(y => y.CreateAsync(newCar)).Should().ThrowAsync<DbUpdateException>();
        }

        [Fact]
        public async Task CreateAndGet_BasicTest()
        {
            ICarService carService = Resolve<ICarService>();
            string suffix = TimeOnly.FromDateTime(DateTime.Now).ToString("hhmmssfffffff");

            Car entity = CarsCatalog.CitroenC4(suffix);
            var result = await carService.CreateAsync(entity);

            result.Result.Should().Be(CommandState.Success);
            result.Entity.Should().NotBeNull();

            result.Entity!.Id.Should().Be((long)result.Entity.DatabaseID);

            var getResult = await carService.QueryAsync(new BasicCarQuery() { ID = result.Entity.Id }, includeCount: true);

            getResult.TotalItems.Should().Be(1);
            getResult.Results.Should().NotBeEmpty().And.HaveCount(1);
            getResult.Results.Should().SatisfyRespectively(
                first =>
                {
                    first.Id.Should().BeGreaterThan(0);
                    first.Name.Should().Be(entity.Name);
                });
        }

        [Fact]
        public async Task CreateAndGet_ListTest()
        {
            (ICarService carService, _, Brand citroenBrand, string suffix, _) = await CreateCarsAsync();

            var getResult = await carService.QueryAsync(new BasicCarQuery() { NameSuffix = suffix }, includeCount: true);

            getResult.TotalItems.Should().Be(2);

            getResult.Results.Select(x => x.Id).Should().BeInAscendingOrder();

            var getResultFluent = await carService.QueryAsync(QueryBuilder.Create<BasicCarQuery, Car>().Where(x => x.Name.EndsWith(suffix)).Build(), includeCount: true);

            getResultFluent.Results.Select(x => x.Id).Should().BeEquivalentTo(getResult.Results.Select(x => x.Id));

            IList<Car> listAllResult = await carService.ListAllAsync();
            listAllResult.Count(x => x.Name.EndsWith(suffix)).Should().Be(2);

            var listAllWithCountryResult = await carService.ListAllAsync([x => x.Brand.Country]);
            listAllWithCountryResult.Select(x => x.Brand.Country.Name).Should().HaveCountGreaterThanOrEqualTo(1);
        }

        [Fact]
        public async Task CreateAndGet_ListWithIncludeStringTest()
        {
            (ICarService carService, _, Brand citroenBrand, string suffix, _) = await CreateCarsAsync();

            var getResult = await carService.QueryAsync(new BasicCarQuery() { IncludeStrings = ["Brand.Country"], NameSuffix = suffix }, includeCount: true);

            getResult.TotalItems.Should().Be(2);

            getResult.Results.Select(x => x.Id).Should().BeInAscendingOrder();

            getResult.Results.Should().OnlyContain(x => x.Brand.Country != null);

            var getResultFluent = await carService.QueryAsync(QueryBuilder.Create<BasicCarQuery, Car>().Where(x => x.Name.EndsWith(suffix)).Include("Brand.Country").Build(), includeCount: true);

            getResultFluent.Results.Select(x => x.Id).Should().BeInAscendingOrder();

            getResultFluent.Results.Should().OnlyContain(x => x.Brand.Country != null);

            getResultFluent.Results.Select(x => x.Id).Should().BeEquivalentTo(getResult.Results.Select(x => x.Id));
        }

        [Fact]
        public async Task CreateAndGet_ListWithIncludeTest()
        {
                (ICarService carService, _, Brand citroenBrand, string suffix, _) = await CreateCarsAsync();

            var getResult = await carService.QueryAsync
                (new BasicCarQuery() { Includes = [x => x.Brand.Country], NameSuffix = suffix }, includeCount: true);

            getResult.TotalItems.Should().Be(2);

            getResult.Results.Select(x => x.Id).Should().BeInAscendingOrder();

            getResult.Results.Should().OnlyContain(x => x.Brand.Country != null);

            var getResultFluent = await carService.QueryAsync(QueryBuilder.Create<BasicCarQuery, Car>().Where(x => x.Name.EndsWith(suffix)).Include(x => x.Brand.Country).Build(), includeCount: true);

            getResultFluent.TotalItems.Should().Be(2);

            getResultFluent.Results.Select(x => x.Id).Should().BeInAscendingOrder();

            getResultFluent.Results.Should().OnlyContain(x => x.Brand.Country != null);
        }

        public class SmallCarInfo : IMapFrom<Car>
        {
            public long ID { get; set; }
            public string? Name { get; set; }
        }

        [Fact]
        public async Task CreateAndGet_ListWithMappingTest()
        {
            (ICarService carService, _, Brand citroenBrand, string suffix, _) = await CreateCarsAsync();

            var getResult = await carService.QueryWithProjectionAsync<SmallCarInfo>(new BasicCarQuery() { NameSuffix = suffix }, includeCount: true);

            getResult.TotalItems.Should().Be(2);

            getResult.Results.Should().OnlyContain(x => x.Name != null);
        }

        public class SmallCarWithCentury : IMapFrom<Car>
        {
            public long ID { get; set; }
            public string? Name { get; set; }
            public bool CurrentCentury { get; set; }
            public int ReleaseYear { get; set; }
        }

        [Fact]
        public async Task CreateAndGet_SelectWithFunctionTest()
        {
            (ICarService carService, _, Brand citroenBrand, string suffix, _) = await CreateCarsAsync();

            var dbCreationResult = await carService.CreateAsync(new Car() { Name = "2CV" + suffix, ReleaseYear = 1965, BrandId = citroenBrand.Id });
            dbCreationResult.Result.Should().Be(CommandState.Success);

            var getResult = await carService.QueryAsync(QueryBuilder.Create<BasicCarQuery, Car>().Where(Car.ModernCarFunction).And(x => x.Name.EndsWith(suffix)).Build());

            getResult.Results.Where(x => x.ReleaseYear >= 2000).Should().HaveCount(getResult.Results.Count(x => x.CurrentCentury));
            getResult.Results.Where(x => x.ReleaseYear < 2000).Should().HaveCount(getResult.Results.Count(x => !x.CurrentCentury));
        }

        [Fact]
        public async Task CreateAndGet_ListWithFunctionTest()
        {
            (ICarService carService, _, Brand citroenBrand, string suffix, _) = await CreateCarsAsync();

            var dbCreationResult = await carService.CreateAsync(new Car() { Name = "2CV" + suffix, ReleaseYear = 1965, BrandId = citroenBrand.Id });
            dbCreationResult.Result.Should().Be(CommandState.Success);

            var getResult = await carService.QueryAsync
                (new BasicCarQuery() { IsModernCar = true, NameSuffix = suffix }, includeCount: true);

            getResult.TotalItems.Should().Be(2);
        }

        public class SmallCarInfoWithBrand : IMapFrom<Car>
        {
            public long ID { get; set; }
            public string? Name { get; set; }
            public string? BrandName { get; set; } //Automatic mapping with Brand.Name
            public string? BrandCountryName { get; set; }
        }

        [Fact]
        public async Task CreateAndGet_ListWithMapping2LevelTest()
        {
            (ICarService carService, _, Brand citroenBrand, string suffix, _) = await CreateCarsAsync();

            var getResult = await carService.QueryWithProjectionAsync<SmallCarInfoWithBrand>(new BasicCarQuery() { NameSuffix = suffix }, includeCount: true);

            getResult.TotalItems.Should().Be(2);

            getResult.Results.Select(x => x.ID).Should().BeInAscendingOrder();

            getResult.Results.Should().OnlyContain(x => x.Name != null);
            getResult.Results.Select(x => x.Name).Should().AllSatisfy(x => x.EndsWith(suffix));
            getResult.Results.Should().OnlyContain(x => x.BrandName != null);
            getResult.Results.Select(x => x.BrandName).Should().AllSatisfy(x => x.EndsWith(suffix));
            getResult.Results.Select(x => x.BrandCountryName).Should().AllSatisfy(x => x.Equals("FR"));
        }

        [Fact]
        public async Task CreateAndGet_ListWithPagingTest()
        {
                (ICarService carService, _, Brand citroenBrand, string suffix, _) = await CreateCarsAsync();

            const int pageSize = 1;
            var getResult = await carService.QueryAsync(new BasicCarQuery() { Skip = 1, Take = pageSize, NameSuffix = suffix }, includeCount: true);

            getResult.TotalItems.Should().Be(2);
            getResult.Results.Count.Should().Be(1);

            var getResultFluent =
                await carService.QueryAsync(QueryBuilder.Create<BasicCarQuery, Car>().Where(x => x.Name.EndsWith(suffix)).OrderBy(x => x.Id)
                .Take(pageSize).Skip(1).Build(), includeCount: true);

            getResultFluent.TotalItems.Should().Be(2);
            getResultFluent.Results.Count.Should().Be(1);
        }

        [Fact]
        public async Task OrderString_Tests()
        {
            (ICarService carService, _, Brand citroenBrand, string suffix, _) = await CreateCarsAsync();

            var getResult = await carService.QueryAsync(new BasicCarQuery()
            {
                NameSuffix = suffix,
                OrderByStrings = [new OrderBySpecification<string>(nameof(Car.ReleaseYear), false)]
            });

            getResult.Results.Select(x => x.ReleaseYear).Should().BeInAscendingOrder();

            getResult = await carService.QueryAsync(new BasicCarQuery()
            {
                OrderByStrings = [new OrderBySpecification<string>(nameof(Car.ReleaseYear), true)]
            });

            getResult.Results.Select(x => x.ReleaseYear).Should().BeInDescendingOrder();

            var getResultFluent = await carService.QueryAsync(QueryBuilder.Create<BasicCarQuery, Car>().OrderBy(nameof(Car.ReleaseYear)).Build());

            getResultFluent.Results.Select(x => x.ReleaseYear).Should().BeInAscendingOrder();
        }

        [Fact]
        public async Task OrderWith2LevelString_Test()
        {
            (ICarService carService, _, Brand citroenBrand, string suffix, _) = await CreateCarsAsync();

            var getResult = await carService.QueryAsync(new BasicCarQuery()
            {
                OrderByStrings = [new("Brand.Country.Name", false)],
                IncludeStrings = ["Brand.Country"]
            });

            getResult.Results.Select(x => (int)x.Brand.Country.Name[0]).Should().BeInAscendingOrder();

            getResult = await carService.QueryAsync(new BasicCarQuery()
            {
                OrderByStrings = [new("Brand.Country.Name", true)],
                IncludeStrings = ["Brand.Country"]
            });

            getResult.Results.Select(x => (int)x.Brand.Country.Name[0]).Should().BeInDescendingOrder();

            var getResultFluent = await carService.QueryAsync(QueryBuilder.Create<BasicCarQuery, Car>().OrderBy("Brand.Country.Name", true).Include("Brand.Country").Build());

            getResultFluent.Results.Select(x => (int)x.Brand.Country.Name[0]).Should().BeInDescendingOrder();
        }

        [Fact]
        public async Task OrderFunction_Tests()
        {
            (ICarService carService, _, Brand citroenBrand, string suffix, _) = await CreateCarsAsync();

            var getResult = await carService.QueryAsync(new BasicCarQuery()
            {
                OrderBy = [new OrderBySpecification<Expression<Func<Car, object>>>(x => x.ReleaseYear)]
            });

            getResult.Results.Select(x => x.ReleaseYear).Should().BeInAscendingOrder();

            getResult = await carService.QueryAsync(new BasicCarQuery()
            {
                OrderBy = [new OrderBySpecification<Expression<Func<Car, object>>>(x => x.ReleaseYear, true)]
            });

            getResult.Results.Select(x => x.ReleaseYear).Should().BeInDescendingOrder();

            var getResultFluent = await carService.QueryAsync(QueryBuilder.Create<BasicCarQuery, Car>().OrderBy(x => x.ReleaseYear).Include("Brand.Country").Build());

            getResultFluent.Results.Select(x => x.ReleaseYear).Should().BeInAscendingOrder();

            getResultFluent = await carService.QueryAsync(QueryBuilder.Create<BasicCarQuery, Car>().OrderBy(x => x.ReleaseYear, true).Include("Brand.Country").Build());

            getResultFluent.Results.Select(x => x.ReleaseYear).Should().BeInDescendingOrder();
        }

        [Fact]
        public async Task OrderFunction2Level_Tests()
        {
            string suffix = TimeOnly.FromDateTime(DateTime.Now).ToString("hhmmssfffffff");

            ICarService carService = Resolve<ICarService>();
            IBrandService brandService = Resolve<IBrandService>();

            Brand citroenBrand = CarsCatalog.Citroen(suffix);
            await brandService.CreateAsync(citroenBrand);

            Car newCar = new()
            {
                Name = "C7" + suffix,
                ReleaseYear = 2011,
                BrandId = citroenBrand.Id
            };

            _ = await carService.CreateAsync(newCar);

            Car newCar2 = new()
            {
                Name = "C6" + suffix,
                ReleaseYear = 2011,
                BrandId = citroenBrand.Id
            };

            _ = await carService.CreateAsync(newCar2);

            Car newCar3 = new()
            {
                Name = "C8" + suffix,
                ReleaseYear = 2011,
                BrandId = citroenBrand.Id
            };

            _ = await carService.CreateAsync(newCar3);

            var getResult = await carService.QueryAsync(new BasicCarQuery()
            {
                NameSuffix = suffix,
                OrderBy = [
                    new(x => x.ReleaseYear),
                    new(x => x.Name)
                ]
            });

            getResult.Results.Select(x => x.ReleaseYear).Should().BeInAscendingOrder();
            getResult.Results.Where(x => x.ReleaseYear == newCar.ReleaseYear).Select(x => x.ReleaseYear).Should().BeInAscendingOrder();
            getResult.Results.Where(x => x.ReleaseYear == 2011).Select(x => x.Name).Should().BeEquivalentTo(["C6" + suffix, "C7" + suffix, "C8" + suffix]);

            var getResultFluent = await carService.QueryAsync(QueryBuilder.Create<BasicCarQuery, Car>().Where(x => x.Name.EndsWith(suffix)).OrderBy(x => x.ReleaseYear).ThenOrderBy(x => x.Name).Build());
            getResultFluent.Results.Select(x => x.ReleaseYear).Should().BeInAscendingOrder();
            getResultFluent.Results.Where(x => x.ReleaseYear == newCar.ReleaseYear).Select(x => x.ReleaseYear).Should().BeInAscendingOrder();
            getResultFluent.Results.Where(x => x.ReleaseYear == newCar.ReleaseYear && x.ReleaseYear == 2011).Select(x => x.Name).Should().BeInAscendingOrder();

            BasicCarQuery specification = new() { NameSuffix = suffix };
            specification.AddOrder(new OrderBySpecification<Expression<Func<Car, object>>>(x => x.ReleaseYear, false));
            specification.AddOrder(new OrderBySpecification<Expression<Func<Car, object>>>(x => x.Name, true));
            getResult = await carService.QueryAsync(specification);

            getResult.Results.Select(x => x.ReleaseYear).Should().BeInAscendingOrder();
            getResult.Results.Where(x => x.ReleaseYear == newCar.ReleaseYear).Select(x => x.ReleaseYear).Should().BeInDescendingOrder();
            getResult.Results.Where(x => x.ReleaseYear == newCar.ReleaseYear && x.ReleaseYear == 2011).Select(x => x.Name).Should().BeInDescendingOrder();

            getResultFluent = await carService.QueryAsync(QueryBuilder.Create<BasicCarQuery, Car>().Where(x => x.Name.EndsWith(suffix))
                .OrderBy(x => x.ReleaseYear).ThenOrderBy(x => x.Name, true).Build());
            getResultFluent.Results.Select(x => x.ReleaseYear).Should().BeInAscendingOrder();
            getResultFluent.Results.Where(x => x.ReleaseYear == newCar.ReleaseYear).Select(x => x.ReleaseYear).Should().BeInDescendingOrder();
            getResultFluent.Results.Where(x => x.ReleaseYear == newCar.ReleaseYear && x.ReleaseYear == 2011).Select(x => x.Name).Should().BeInDescendingOrder();

            getResult = await carService.QueryAsync(new BasicCarQuery()
            {
                NameSuffix = suffix,
                OrderBy = [
                    new OrderBySpecification<Expression<Func<Car, object>>>(x => x.ReleaseYear) ,
                    new OrderBySpecification<Expression<Func<Car, object>>>(x => x.Name , true)
                ]
            });

            getResult.Results.Select(x => x.ReleaseYear).Should().BeInAscendingOrder();
            getResult.Results.Where(x => x.ReleaseYear == newCar.ReleaseYear).Select(x => x.ReleaseYear).Should().BeInDescendingOrder();
            getResult.Results.Where(x => x.ReleaseYear == newCar.ReleaseYear && x.ReleaseYear == 2011).Select(x => x.Name).Should().BeInDescendingOrder();
        }

        [Fact]
        public async Task BrandGroupByCarCount_BasicTest()
        {
            (ICarService carService, _, Brand citroenBrand, string suffix, _) = await CreateCarsAsync();

            var getResult = await carService.GetBrandWithCountAsync();

            getResult.Should().HaveCountGreaterThan(0);
            getResult.Where(x => x.BrandName == citroenBrand.Name).Should().HaveCount(1);
            getResult.First(x => x.BrandName == citroenBrand.Name).CarCount.Should().Be(2);
        }

        [Fact]
        public async Task QueryBuilder_BasicTest()
        {
            (ICarService carService, _, Brand citroenBrand, string suffix, _) = await CreateCarsAsync();

            var getResult = await carService.QueryAsync(
                QueryBuilder.Create(new BasicCarQuery()).Where(x => x.ReleaseYear >= 2010).OrderBy("ReleaseYear", true).Build()
            );

            getResult.Results.Should().NotBeEmpty();
        }

        [Fact]
        public async Task StringSelector_BasicTest()
        {
            (ICarService carService, _, _, string suffix, _) = await CreateCarsAsync();

            QueryResult<Car> getResult = await carService.QueryAsync(new BasicCarQuery() { Expression = "x => (x.Name == null ? \"\" : x.Name).Contains(\"" + suffix + "\")" });

            getResult.Results.Should().NotBeEmpty();
            getResult.Results.Count.Should().Be(2);
        }

        [Fact]
        public async Task UpdatePropertiesAsync_Test()
        {
            (ICarService carService, _, _, string suffix, _) = await CreateCarsAsync();

            Car? firstCar = (await carService.GetFirstByQueryAsync(new BasicCarQuery() { NameSuffix = suffix}));

            firstCar.Should().NotBeNull();

            const string nameSuffix = "XXXX";

            firstCar!.Name += nameSuffix;

            CommandResult<Car> updateResult = await carService.UpdatePropertiesAsync(firstCar, [nameof(Car.Name)]);

            updateResult.Result.Should().Be(CommandState.Success);

            Car? carAfterUpdate = (await carService.GetFirstByQueryAsync(new BasicCarQuery() { ID = firstCar.Id}));

            carAfterUpdate.Should().NotBeNull();
            carAfterUpdate!.Name.Should().EndWith(nameSuffix);
        }

        [Fact]
        public async Task GetFirstByQueryAsync_BasicTest()
        {
            (ICarService carService, _, Brand citroenBrand, string suffix, _) = await CreateCarsAsync();

            Car result =
                await carService.GetFirstByQueryAsync(
                    QueryBuilder.Create(new BasicCarQuery()).Where(x => x.Brand.Name == citroenBrand.Name)
                    .OrderBy("Id", true)
                    .Build());

            result.Should().NotBeNull();
            result.Name.Should().Be("C5" + suffix);

            result =
                await carService.GetFirstByQueryAsync(
                    QueryBuilder.Create(new BasicCarQuery()).Where(x => x.Brand.Name == citroenBrand.Name)
                    .OrderBy("Id", false)
                    .Build());

            result.Should().NotBeNull();
            result.Name.Should().Be("C4" + suffix);

            result =
            await carService.GetFirstByQueryAsync(
                QueryBuilder.Create(new BasicCarQuery()).Where(x => x.Brand.Name == "XXXX")
                .Build());

            result.Should().BeNull();
        }

        [Fact]
        public async Task GetFirstByQueryWithProjectionAsync_BasicTest()
        {
            (ICarService carService, _, Brand citroenBrand, string suffix, _) = await CreateCarsAsync();

            SmallCarInfo result =
                await carService.GetFirstByQueryWithProjectionAsync<SmallCarInfo>(
                    QueryBuilder.Create(new BasicCarQuery()).Where(x => x.Brand.Name == citroenBrand.Name)
                    .OrderBy("Id", true)
                    .Build());

            result.Should().NotBeNull();
            result.Name.Should().Be("C5" + suffix);

            result =
                 await carService.GetFirstByQueryWithProjectionAsync<SmallCarInfo>(
                    QueryBuilder.Create(new BasicCarQuery()).Where(x => x.Brand.Name == citroenBrand.Name)
                    .OrderBy("Id", false)
                    .Build());

            result.Should().NotBeNull();
            result.Name.Should().Be("C4" + suffix);

            result =
                 await carService.GetFirstByQueryWithProjectionAsync<SmallCarInfo>(
                QueryBuilder.Create(new BasicCarQuery()).Where(x => x.Brand.Name == "XXXX")
                .Build());

            result.Should().BeNull();
        }

        //[Fact]
        public async Task EntitiesWithDifferentState_Test()
        {
            (ICarService carService, _, Brand citroenBrand, string suffix, _) = await CreateCarsAsync();

            Car newCar = new()
            {
                Name = "C3",
                ReleaseYear = 2011,
                Brand = new Brand()
                {
                    Name = "Citroen",
                    Country = new Country()
                    {
                        Name = "France",
                        CountryCode = "FR"
                    }
                }
            };

            var result = await carService.CreateAsync(newCar);

            result.Result.Should().Be(CommandState.Success);

            Car newCar2 = new()
            {
                Name = "C4",
                ReleaseYear = 2011,
                BrandId = newCar.BrandId,
                Brand = new Brand()
                {
                    Name = "Citroen",
                    Id = newCar.BrandId
                }
            };

            result = await carService.CreateAsync(newCar2);

            result.Result.Should().Be(CommandState.Error);
        }
    }
}

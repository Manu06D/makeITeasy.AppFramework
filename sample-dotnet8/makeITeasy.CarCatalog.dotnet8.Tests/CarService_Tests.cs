using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

using FluentAssertions;

using makeITeasy.AppFramework.Core.Commands;
using makeITeasy.AppFramework.Core.Interfaces;
using makeITeasy.AppFramework.Models;
using makeITeasy.CarCatalog.dotnet8.Core.Services.Interfaces;
using makeITeasy.CarCatalog.dotnet8.Core.Services.Queries.CarQueries;
using makeITeasy.CarCatalog.dotnet8.Tests.Catalogs;
using makeITeasy.CarCatalog.dotnet8.Infrastructure.Data;
using makeITeasy.CarCatalog.dotnet8.Models;

using Microsoft.EntityFrameworkCore;

using Xunit;
using makeITeasy.AppFramework.Core.Queries;

namespace makeITeasy.CarCatalog.dotnet8.Tests
{
    public class CarService_Tests : UnitTestAutofacService<ServiceRegistrationAutofacModule>
    {
        private ICarService carService;
        private List<Car> carList;

        public CarService_Tests()
        {
            carService = Resolve<ICarService>();
            var t = Resolve<CarCatalogContext>();

            t.Database.EnsureCreated();
        }

        ~CarService_Tests()
        {
            carService = null;
        }

        public void CreateCarCatalog()
        {
            carList = TestCarsCatalog.SaveCarsInDB(carService);
        }

        [Fact]
        public void IsValid_InValidObjectTest()
        {
            var newCar = new Car
            {
                Name = "x"
            };

            carService.Validate(newCar).Should().BeFalse();
        }

        [Fact]
        public void IsValid_ValidObjectTest()
        {
            var newCar = new Car
            {
                Name = "xxxx"
            };

            carService.Validate(newCar).Should().BeTrue();
        }

        [Fact]
        public async Task Create_InValidObjectTest()
        {
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
            var newCar = new Car
            {
                Name = "xxx"
            };

            carService.Invoking(y => y.CreateAsync(newCar)).Should().ThrowAsync<DbUpdateException>();
        }

        [Fact]
        public async Task CreateAndGet_BasicTest()
        {
            Car newCar = new Car()
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

            result.Entity.Id.Should().Be((long)result.Entity.DatabaseID);

            var getResult = await carService.QueryAsync(new BaseCarQuery() { ID = result.Entity.Id }, includeCount: true);

            getResult.TotalItems.Should().Be(1);
            getResult.Results.Should().NotBeEmpty().And.HaveCount(1);
            getResult.Results.Should().SatisfyRespectively(
                first =>
                {
                    first.Id.Should().BeGreaterThan(0);
                    first.Name.Should().Be(newCar.Name);
                });
        }

        [Fact]
        public async Task CreateAndGet_ListTest()
        {
            CreateCarCatalog();
            
            var getResult = await carService.QueryAsync(new BaseCarQuery(), includeCount: true);

            getResult.TotalItems.Should().Be(carList.Count);

            getResult.Results.Select(x => x.Id).Should().BeInAscendingOrder();

            var getResultFluent = await carService.QueryAsync(QueryBuilder.Create<BaseCarQuery, Car>().Build(), includeCount: true);

            getResultFluent.Results.Select(x => x.Id).Should().BeEquivalentTo(getResult.Results.Select(x => x.Id));

            var listAllResult = await carService.ListAllAsync();
            listAllResult.Count.Should().Be(carList.Count);

            var listAllWithCountryResult = await carService.ListAllAsync(new List<Expression<Func<Car, object>>>() { x => x.Brand.Country});
            listAllWithCountryResult.Select(x => x.Brand.Country.Name).Should().HaveCountGreaterThan(1);
        }

        [Fact]
        public async Task CreateAndGet_ListWithIncludeStringTest()
        {
            CreateCarCatalog();
            
            var getResult = await carService.QueryAsync(new BaseCarQuery() { IncludeStrings = new List<string>() { "Brand.Country" } }, includeCount: true);

            getResult.TotalItems.Should().Be(carList.Count);

            getResult.Results.Select(x => x.Id).Should().BeInAscendingOrder();

            getResult.Results.Should().OnlyContain(x => x.Brand.Country != null);

            var getResultFluent = await carService.QueryAsync(QueryBuilder.Create<BaseCarQuery, Car>().Include("Brand.Country").Build(), includeCount: true);

            getResultFluent.Results.Select(x => x.Id).Should().BeInAscendingOrder();

            getResultFluent.Results.Should().OnlyContain(x => x.Brand.Country != null);

            getResultFluent.Results.Select(x => x.Id).Should().BeEquivalentTo(getResult.Results.Select(x => x.Id));
        }

        [Fact]
        public async Task CreateAndGet_ListWithIncludeTest()
        {
            CreateCarCatalog();

            var getResult = await carService.QueryAsync
                (new BaseCarQuery() { Includes = new List<Expression<Func<Car, object>>>() { x => x.Brand.Country } }, includeCount: true);

            getResult.TotalItems.Should().Be(carList.Count);

            getResult.Results.Select(x => x.Id).Should().BeInAscendingOrder();

            getResult.Results.Should().OnlyContain(x => x.Brand.Country != null);

            var getResultFluent = await carService.QueryAsync(QueryBuilder.Create<BaseCarQuery, Car>().Include(x => x.Brand.Country).Build(), includeCount: true);

            getResultFluent.TotalItems.Should().Be(carList.Count);

            getResultFluent.Results.Select(x => x.Id).Should().BeInAscendingOrder();

            getResultFluent.Results.Should().OnlyContain(x => x.Brand.Country != null);
        }

        public class SmallCarInfo : IMapFrom<Car>
        {
            public long ID { get; set; }
            public string Name { get; set; }
        }

        [Fact]
        public async Task CreateAndGet_ListWithMappingTest()
        {
            CreateCarCatalog();

            var getResult = await carService.QueryWithProjectionAsync<SmallCarInfo>(new BaseCarQuery() { }, includeCount: true);

            getResult.TotalItems.Should().Be(carList.Count);

            getResult.Results.Should().OnlyContain(x => x.Name != null);
        }

        public class SmallCarWithCentury : IMapFrom<Car>
        {
            public long ID { get; set; }
            public string Name { get; set; }
            public bool CurrentCentury { get; set; }
            public int ReleaseYear { get; set; }
        }

        [Fact]
        public async Task CreateAndGet_SelectWithFunctionTest()
        {
            var getResult = await carService.QueryAsync(QueryBuilder.Create<BaseCarQuery, Car>().Where(Car.ModernCarFunction).Build());

            getResult.Results.Where(x => x.ReleaseYear >= 2000).Should().HaveCount(getResult.Results.Count(x => x.CurrentCentury));
            getResult.Results.Where(x => x.ReleaseYear < 2000).Should().HaveCount(getResult.Results.Count(x => !x.CurrentCentury));
        }


        [Fact]
        public async Task CreateAndGet_ListWithFunctionTest()
        {
            CreateCarCatalog();

            var getResult = await carService.QueryAsync
                (new BaseCarQuery() { IsModernCar = true }, includeCount: true);

            getResult.TotalItems.Should().Be(carList.Count(x => x.ReleaseYear > 2000));
        }

        public class SmallCarInfoWithBrand : IMapFrom<Car>
        {
            public long ID { get; set; }
            public string Name { get; set; }
            public string BrandName { get; set; } //Automatic mapping with Brand.Name
            public string BrandCountryName { get; set; }
        }

        [Fact]
        public async Task CreateAndGet_ListWithMapping2LevelTest()
        {
            CreateCarCatalog();

            var getResult = await carService.QueryWithProjectionAsync<SmallCarInfoWithBrand>(new BaseCarQuery() { }, includeCount: true);

            getResult.TotalItems.Should().Be(carList.Count);

            getResult.Results.Select(x => x.ID).Should().BeInAscendingOrder();

            getResult.Results.Should().OnlyContain(x => x.Name != null);
            getResult.Results.Should().OnlyContain(x => x.BrandName != null);
        }

        [Fact]
        public async Task CreateAndGet_ListWithPagingTest()
        {
            CreateCarCatalog();

            const int pageSize = 10;
            var getResult = await carService.QueryAsync(new BaseCarQuery() { Skip = 5, Take = pageSize }, includeCount: true);

            getResult.TotalItems.Should().Be(carList.Count);
            getResult.Results.Count.Should().Be(Math.Min(carList.Count, pageSize));

            var getResultFluent = await carService.QueryAsync(QueryBuilder.Create<BaseCarQuery, Car>().OrderBy(x => x.Id).Take(pageSize).Skip(5).Build(), includeCount: true);

            getResultFluent.TotalItems.Should().Be(carList.Count);
            getResultFluent.Results.Count.Should().Be(Math.Min(carList.Count, pageSize));
        }

        [Fact]
        public async Task OrderString_Tests()
        {
            var getResult = await carService.QueryAsync(new BaseCarQuery()
            {
                OrderByStrings = new List<OrderBySpecification<string>>() { new OrderBySpecification<string>(nameof(Car.ReleaseYear), false) }
            });

            getResult.Results.Select(x => x.ReleaseYear).Should().BeInAscendingOrder();

            getResult = await carService.QueryAsync(new BaseCarQuery()
            {
                OrderByStrings = new List<OrderBySpecification<string>>() { new OrderBySpecification<string>(nameof(Car.ReleaseYear), true) }
            });

            getResult.Results.Select(x => x.ReleaseYear).Should().BeInDescendingOrder();

            var getResultFluent = await carService.QueryAsync(QueryBuilder.Create<BaseCarQuery, Car>().OrderBy(nameof(Car.ReleaseYear)).Build());

            getResultFluent.Results.Select(x => x.ReleaseYear).Should().BeInAscendingOrder();

        }

        [Fact]
        public async Task OrderWith2LevelString_Test()
        {
            var getResult = await carService.QueryAsync(new BaseCarQuery()
            {
                OrderByStrings = new List<OrderBySpecification<string>>() { new OrderBySpecification<string>("Brand.Country.Name", false) },
                IncludeStrings = new List<string> { "Brand.Country" }
            });

            getResult.Results.Select(x => (int)x.Brand.Country.Name.First()).Should().BeInAscendingOrder();

            getResult = await carService.QueryAsync(new BaseCarQuery()
            {
                OrderByStrings = new List<OrderBySpecification<string>>() { new OrderBySpecification<string>("Brand.Country.Name", true) },
                IncludeStrings = new List<string> { "Brand.Country" }
            });

            getResult.Results.Select(x => (int)x.Brand.Country.Name.First()).Should().BeInDescendingOrder();

            var getResultFluent = await carService.QueryAsync(QueryBuilder.Create<BaseCarQuery, Car>().OrderBy("Brand.Country.Name", true).Include("Brand.Country").Build());

            getResultFluent.Results.Select(x => (int)x.Brand.Country.Name.First()).Should().BeInDescendingOrder();
        }

        [Fact]
        public async Task OrderFunction_Tests()
        {

            var getResult = await carService.QueryAsync(new BaseCarQuery()
            {
                OrderBy = new List<OrderBySpecification<Expression<Func<Car, object>>>>() { new OrderBySpecification<Expression<Func<Car, object>>>(x => x.ReleaseYear) }
            });

            getResult.Results.Select(x => x.ReleaseYear).Should().BeInAscendingOrder();


            getResult = await carService.QueryAsync(new BaseCarQuery()
            {
                OrderBy = new List<OrderBySpecification<Expression<Func<Car, object>>>>() { new OrderBySpecification<Expression<Func<Car, object>>>(x => x.ReleaseYear, true) }
            });

            getResult.Results.Select(x => x.ReleaseYear).Should().BeInDescendingOrder();

            var getResultFluent = await carService.QueryAsync(QueryBuilder.Create<BaseCarQuery, Car>().OrderBy(x => x.ReleaseYear).Include("Brand.Country").Build());


            getResultFluent.Results.Select(x => x.ReleaseYear).Should().BeInAscendingOrder();

            getResultFluent = await carService.QueryAsync(QueryBuilder.Create<BaseCarQuery, Car>().OrderBy(x => x.ReleaseYear, true).Include("Brand.Country").Build());

            getResultFluent.Results.Select(x => x.ReleaseYear).Should().BeInDescendingOrder();
        }


        [Fact]
        public async Task OrderFunction2Level_Tests()
        {
            Car newCar = new Car()
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

            _ = await carService.CreateAsync(newCar);

            Car newCar2 = new Car()
            {
                Name = "B2",
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

            _ = await carService.CreateAsync(newCar2);

            Car newCar3 = new Car()
            {
                Name = "A2",
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

            _ = await carService.CreateAsync(newCar3);

            var getResult = await carService.QueryAsync(new BaseCarQuery()
            {
                OrderBy = new List<OrderBySpecification<Expression<Func<Car, object>>>>() {
                    new OrderBySpecification<Expression<Func<Car, object>>>(x => x.ReleaseYear ) ,
                    new OrderBySpecification<Expression<Func<Car, object>>>(x => x.Name)
                }
            });

            getResult.Results.Select(x => x.ReleaseYear).Should().BeInAscendingOrder();
            getResult.Results.Where(x => x.ReleaseYear == newCar.ReleaseYear).Select(x => x.Name).Should().BeInAscendingOrder();

            var getResultFluent = await carService.QueryAsync(QueryBuilder.Create<BaseCarQuery, Car>().OrderBy(x => x.ReleaseYear).ThenOrderBy(x => x.Name).Build());
            getResultFluent.Results.Select(x => x.ReleaseYear).Should().BeInAscendingOrder();
            getResultFluent.Results.Where(x => x.ReleaseYear == newCar.ReleaseYear).Select(x => x.Name).Should().BeInAscendingOrder();

            BaseCarQuery specification = new BaseCarQuery() { };
            specification.AddOrder(new OrderBySpecification<Expression<Func<Car, object>>>(x => x.ReleaseYear, false));
            specification.AddOrder(new OrderBySpecification<Expression<Func<Car, object>>>(x => x.Name, true));
            getResult = await carService.QueryAsync(specification);

            getResult.Results.Select(x => x.ReleaseYear).Should().BeInAscendingOrder();
            getResult.Results.Where(x => x.ReleaseYear == newCar.ReleaseYear).Select(x => x.Name).Should().BeInDescendingOrder();

            getResultFluent = await carService.QueryAsync(QueryBuilder.Create<BaseCarQuery, Car>().OrderBy(x => x.ReleaseYear).ThenOrderBy(x => x.Name, true).Build());
            getResultFluent.Results.Select(x => x.ReleaseYear).Should().BeInAscendingOrder();
            getResultFluent.Results.Where(x => x.ReleaseYear == newCar.ReleaseYear).Select(x => x.Name).Should().BeInDescendingOrder();

            getResult = await carService.QueryAsync(new BaseCarQuery()
            {
                OrderBy = new List<OrderBySpecification<Expression<Func<Car, object>>>>() {
                    new OrderBySpecification<Expression<Func<Car, object>>>(x => x.ReleaseYear) ,
                    new OrderBySpecification<Expression<Func<Car, object>>>(x => x.Name , true)
                }
            });

            getResult.Results.Select(x => x.ReleaseYear).Should().BeInAscendingOrder();
            getResult.Results.Where(x => x.ReleaseYear == newCar.ReleaseYear).Select(x => x.Name).Should().BeInDescendingOrder();

        }

        [Fact]
        public async Task BrandGroupByCarCount_BasicTest()
        {
            CreateCarCatalog();

            var getResult = await carService.GetBrandWithCountAsync();

            getResult.Should().HaveCountGreaterThan(0);
            getResult.Select(x => x.CarCount).Where(x => x > 1).Should().HaveCountGreaterThan(0);
        }

        [Fact]
        public async Task QueryBuilder_BasicTest()
        {
            CreateCarCatalog();

            var getResult = await carService.QueryAsync(
                QueryBuilder.Create(new BaseCarQuery()).Where(x => x.ReleaseYear >= 2010).OrderBy("ReleaseYear", true).Build()
            );

            getResult.Results.Should().NotBeEmpty();
        }

        [Fact]
        public async Task StringSelector_BasicTest()
        {
            CreateCarCatalog();

            QueryResult<Car> getResult = await carService.QueryAsync(new BaseCarQuery() { StringSelector = "(Name == null ? \"\" : Name).Contains(\"3\")" });

            getResult.Results.Count.Should().Be(TestCarsCatalog.GetCars().Count(x => x.Name.Contains("3")));

            getResult.Results.Should().NotBeEmpty();
        }

        [Fact]
        public async Task UpdatePropertiesAsync_Test()
        {
            CreateCarCatalog();

            Car? firstCar = (await carService.QueryAsync(new BaseCarQuery())).Results.FirstOrDefault();

            const string nameSuffix = "XXXX";

            firstCar.Name += nameSuffix;

            CommandResult<Car> updateResult = await carService.UpdatePropertiesAsync(firstCar, [nameof(Car.Name)]);

            updateResult.Result.Should().Be(CommandState.Success);

            Car? carAfterUpdate = (await carService.QueryAsync(new BaseCarQuery())).Results.FirstOrDefault();

            carAfterUpdate.Name.Should().EndWith(nameSuffix);
        }

        [Fact]
        public async Task RowVersion_Test()
        {
            CreateCarCatalog();

            //This will not work on sql server but work here on sql lite cause lack of support of rowversion
            var firstCar = (await carService.QueryAsync(new BaseCarQuery())).Results.FirstOrDefault();

            var secondCar = (await carService.QueryAsync(new BaseCarQuery())).Results.FirstOrDefault();

            firstCar.Name += "Test";
            await carService.UpdateAsync(firstCar);

            secondCar.Name += " 2";
            var updateResultProperty = await carService.UpdateAsync(secondCar);

            updateResultProperty.Result.Should().Be(CommandState.Success);
        }

        [Fact]
        public async Task GetFirstByQueryAsync_BasicTest()
        {
            CreateCarCatalog();

            Car result =
                await carService.GetFirstByQueryAsync(
                    QueryBuilder.Create(new BaseCarQuery()).Where(x => x.Brand.Name == "Audi")
                    .OrderBy("Id", true)
                    .Build());

            result.Should().NotBeNull();
            result.Name.Should().Be("A3");

            result =
                await carService.GetFirstByQueryAsync(
                    QueryBuilder.Create(new BaseCarQuery()).Where(x => x.Brand.Name == "Audi")
                    .OrderBy("Id", false)
                    .Build());

            result.Should().NotBeNull();
            result.Name.Should().Be("R8");

            result =
            await carService.GetFirstByQueryAsync(
                QueryBuilder.Create(new BaseCarQuery()).Where(x => x.Brand.Name == "XXXX")
                .Build());

            result.Should().BeNull();
        }

        [Fact]
        public async Task GetFirstByQueryWithProjectionAsync_BasicTest()
        {
            CreateCarCatalog();

            SmallCarInfo result =
                await carService.GetFirstByQueryWithProjectionAsync<SmallCarInfo>(
                    QueryBuilder.Create(new BaseCarQuery()).Where(x => x.Brand.Name == "Audi")
                    .OrderBy("Id", true)
                    .Build());

            result.Should().NotBeNull();
            result.Name.Should().Be("A3");

            result =
                 await carService.GetFirstByQueryWithProjectionAsync<SmallCarInfo>(
                    QueryBuilder.Create(new BaseCarQuery()).Where(x => x.Brand.Name == "Audi")
                    .OrderBy("Id", false)
                    .Build());

            result.Should().NotBeNull();
            result.Name.Should().Be("R8");

            result =
                 await carService.GetFirstByQueryWithProjectionAsync<SmallCarInfo>(
                QueryBuilder.Create(new BaseCarQuery()).Where(x => x.Brand.Name == "XXXX")
                .Build());

            result.Should().BeNull();
        }

        //[Fact]
        public async Task EntitiesWithDifferentState_Test()
        {
            Car newCar = new Car()
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

            Car newCar2 = new Car()
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

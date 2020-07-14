using System;
using System.Collections.Generic;
using System.Linq;
using Bogus;
using Bogus.DataSets;
using makeITeasy.CarCatalog.Models;

namespace makeITeasy.CarCatalog.Tests.Catalogs
{
    public static class TestCarsCatalog
    {
        private static Faker<Country> countryGenerator = new Faker<Country>()
            .RuleFor(c => c.Name, f => f.Address.Country())
            .RuleFor(c => c.CountryCode, f => f.Address.CountryCode(Iso3166Format.Alpha2));


        private static Faker<Brand> brandGenerator = new Faker<Brand>()
                .RuleFor(c => c.Name, f => f.Vehicle.Manufacturer())
                .RuleFor(c => c.Country, countryGenerator.Generate(1).First());


        private static Faker<Car> carGenerator = new Faker<Car>()
            .RuleFor(u => u.Name, f => f.Vehicle.Model())
            .RuleFor(u => u.ReleaseYear, f => f.Date.Between(new DateTime(1900, 1, 1), DateTime.Now).Year)
            .RuleFor(u => u.Brand, f => brandGenerator.Generate(1).First())
            ;

        public static Car GetValidCar()
        {
            return carGenerator.Generate(1).First();
        }

        public static List<Car> GetValidCars(int max)
        {
            return carGenerator.Generate(max);
        }
    }
}

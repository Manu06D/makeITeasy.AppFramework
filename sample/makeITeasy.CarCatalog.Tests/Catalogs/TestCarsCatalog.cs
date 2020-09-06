using System;
using System.Collections.Generic;
using System.Linq;
using makeITeasy.CarCatalog.Models;

namespace makeITeasy.CarCatalog.Tests.Catalogs
{
    public class TestCarsCatalog
    {
        private static List<string[]> rawCars = new List<string[]>
        {
            new[]{ "Peugeot", "205", "1993", "France", "FR" },
            new[]{ "Citroen", "C4", "2011", "France", "FR"},
            new[]{ "Peugeot", "2008", "2019", "France", "FR"},
            new[]{ "Tesla", "Model 3", "2017", "United States", "US"},
            new[]{ "Chevrolet", "Corvette", "1953", "United States", "US"},
            new[]{ "Fiat", "500", "1936", "italy", "IT"},
            new[]{ "Ferrari", "F40", "1987", "italy", "IT"},
            new[]{ "Ferrari", "458", "2009", "italy", "IT"},
            new[]{ "Ferrari", "488", "2018", "italy", "IT"},
            new[]{ "Ferrari", "Testarossa", "1984", "italy", "IT"},
            new[]{ "Aston-Martin", "V8 Vantage", "2005", "Great-Britain", "GB"},
            new[]{ "Honda", "Accord", "2013", "Japan", "JP"},
            new[]{ "Audi", "R8", "2015", "Germany", "DE"},
            new[]{ "Audi", "Q3", "2018", "Germany", "DE"},
            new[]{ "Audi", "A3", "2012", "Germany", "DE"},
        };

        public static List<Car> GetCars()
        {

            var rawCountries = rawCars.Select(x => new Country { Name = x[3], CountryCode = x[4] }).GroupBy(x => x.CountryCode).Select(x => x.First()).ToList();

            var rawBrand = rawCars.Select(x => new Brand { Name = x[0], Country = rawCountries.First(y => y.CountryCode == x[4]) }).GroupBy(x => x.Name).Select(x => x.First()).ToList();

            var rawCar = rawCars.Select(x => new Car { Name = x[1], ReleaseYear = int.Parse(x[2]), Brand = rawBrand.First(y => y.Name == x[0]) });

            return rawCar.ToList();
        }
    }
}

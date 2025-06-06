﻿using makeITeasy.CarCatalog.dotnet9.Core.Services.Interfaces;
using makeITeasy.CarCatalog.dotnet9.Models;

namespace makeITeasy.CarCatalog.dotnet9.Tests.Catalogs
{
    public class CarsCatalog
    {
        public static Car CitroenC4(string? suffix = "", Brand? brand = null, int? brandId = null) => new()
        {
            Name = "C4" + suffix,
            ReleaseYear = 2011,
            Brand = brand ?? (brandId.HasValue ? null : Citroen(suffix)),
            BrandId = brandId ?? 0
        };

        public static Car CitroenC5(string? suffix = "", Brand? brand = null, int? brandId = null) => new()
        {
            Name = "C5" + suffix,
            ReleaseYear = 2013,
            Brand = brand ?? (brandId.HasValue ? null : Citroen(suffix)),
            BrandId = brandId ?? 0
        };

        public static Country France => new()
        {
            Name = "France",
            CountryCode = "FR"
        };

        public static Brand Citroen(string? suffix = "", Country? country = null, int? countryId = null) => new()
        {
            Name = "Citroen" + suffix,
            Country = country ?? (countryId.HasValue ? null : France),
            CountryId = countryId ?? 0
        };

        private static readonly List<string[]> rawCars = new List<string[]>
        {
            new[]{ "Peugeot", "205", "1993", "France", "FR" },
            new[]{ "Citroen", "C4", "2011", "France", "FR"},
            new[]{ "Peugeot", "2008", "2019", "France", "FR"},
            new[]{ "Tesla", "Model 3", "2018", "United States", "US"},
            new[]{ "Chevrolet", "Corvette", "1953", "United States", "US"},
            new[]{ "Fiat", "500", "1936", "Italy", "IT"},
            new[]{ "Ferrari", "F40", "1987", "Italy", "IT"},
            new[]{ "Ferrari", "458", "2009", "Italy", "IT"},
            new[]{ "Ferrari", "488", "2018", "Italy", "IT"},
            new[]{ "Ferrari", "Testarossa", "1984", "Italy", "IT"},
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

        public async static Task<List<Car>> SaveCarsInDBAsync(ICarService carService)
        {
            List<Car> carList = GetCars();

            foreach(var car in carList)
            {
                if (car.Brand.Country?.Id > 0)
                {
                    car.Brand.CountryId = car.Brand.Country.Id;
                    car.Brand.Country = null;
                }
                if (car.Brand.Id > 0)
                {
                    car.BrandId = car.Brand.Id;
                    car.Brand = null;
                }

                await carService.CreateAsync(car);
            }

            //carList.ForEach(async x =>
            //{
            //    if (x.Brand.Country?.Id > 0)
            //    {
            //        x.Brand.CountryId = x.Brand.Country.Id;
            //        x.Brand.Country = null;
            //    }

            //    if (x.Brand.Id > 0)
            //    {
            //        x.BrandId = x.Brand.Id;
            //        x.Brand = null;
            //    }

            //    await carService.CreateAsync(x);
            //});

            return carList;
        }
    }
}

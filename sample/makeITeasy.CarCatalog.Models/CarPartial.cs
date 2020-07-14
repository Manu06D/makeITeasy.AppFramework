using System;
using System.Linq.Expressions;
using DelegateDecompiler;

namespace makeITeasy.CarCatalog.Models
{
    public partial class Car
    {
        public override object DatabaseID { get => Id; set => throw new NotImplementedException(); }

        public static Expression<Func<Car, bool>> ModernCarFunction => (x) => x.ReleaseYear > 2000;

        public static Expression<Func<Car, bool>> ItalianCarFunction => (x) => x.Brand.Country.CountryCode == "IT";

        [Computed]
        public bool IsModernCar => ModernCarFunction.Compile()(this);

        //this cannot be used on filter
        [Computed]
        public bool IsSuperModernCar => this.ReleaseYear > 2020;
    }
}

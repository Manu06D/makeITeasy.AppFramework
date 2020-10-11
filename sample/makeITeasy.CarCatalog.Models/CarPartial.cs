using System;
using System.Linq.Expressions;
using DelegateDecompiler;
using makeITeasy.AppFramework.Models;

namespace makeITeasy.CarCatalog.Models
{
    public partial class Car : ITimeTrackingEntity
    {
        public override object DatabaseID { get => Id; }

        public static Expression<Func<Car, bool>> ModernCarFunction => (x) => x.ReleaseYear > 2000;

        public static Expression<Func<Car, bool>> ItalianCarFunction => (x) => x.Brand.Country.CountryCode == "IT";

        [Computed]
        public bool IsModernCar => ModernCarFunction.Compile()(this);

        //this cannot be used on filter
        [Computed]
        public bool IsSuperModernCar => ReleaseYear > 2020;
    }
}

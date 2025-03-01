using System.Linq.Expressions;
using DelegateDecompiler;
using makeITeasy.AppFramework.Models;

namespace makeITeasy.CarCatalog.dotnet9.Models
{
    public partial class Car : ITimeTrackingEntity
    {
        public object DatabaseID => Id;

        public static Expression<Func<Car, bool>> ModernCarFunction => (x) => x.ReleaseYear > 2000;

        public bool IsModernCar => ModernCarFunction.Compile()(this);

        public static Expression<Func<Country, bool>> ItalianCarFunction => (x) => x.CountryCode == "IT";

        [Computed]
        public bool IsItalianCar => ItalianCarFunction.Compile()(Brand?.Country);

        [Computed]
        public bool IsSuperModernCar => ReleaseYear > 2020;

        [Computed]
        public bool CurrentCentury => ReleaseYear >= 2000;

    }
}

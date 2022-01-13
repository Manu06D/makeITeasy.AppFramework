using System.Collections.Generic;
using System.Linq;

using DelegateDecompiler;


namespace makeITeasy.CarCatalog.dotnet6.Models.Collections
{
    public static class CarCollectionExtensions
    {
        [Computed]
        public static int? MinimalReleaseYear(this ICollection<Car> input)
        {
            return input?.Min(x => x.ReleaseYear);
        }
    }
}

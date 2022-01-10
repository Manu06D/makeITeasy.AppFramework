using System.Collections.Generic;
using System.Threading.Tasks;

using makeITeasy.AppFramework.Core.Interfaces;
using makeITeasy.CarCatalog.dotnet6.Models;
using makeITeasy.CarCatalog.dotnet6.Models.Custom;

namespace makeITeasy.CarCatalog.dotnet6.Core.Ports
{
    public interface ICarRepository : IAsyncRepository<Car>
    {
        Task<List<BrandGroupByCarCount>> GroupByBrandAndCountAsync();
    }
}

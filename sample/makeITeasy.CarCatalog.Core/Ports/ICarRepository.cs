using System.Collections.Generic;
using System.Threading.Tasks;

using makeITeasy.AppFramework.Core.Interfaces;
using makeITeasy.CarCatalog.Models;
using makeITeasy.CarCatalog.Models.Custom;

namespace makeITeasy.CarCatalog.Core.Ports
{
    public interface ICarRepository : IAsyncRepository<Car>
    {
        Task<List<BrandGroupByCarCount>> GroupByBrandAndCountAsync();
    }
}

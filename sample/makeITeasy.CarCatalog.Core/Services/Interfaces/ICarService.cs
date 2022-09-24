using System.Collections.Generic;
using System.Threading.Tasks;

using makeITeasy.AppFramework.Core.Interfaces;
using makeITeasy.CarCatalog.Models.Custom;
using makeITeasy.CarCatalog.Models;

namespace makeITeasy.CarCatalog.Core.Services.Interfaces
{
    public interface ICarService : IBaseEntityService<Car>
    {
        Task<List<BrandGroupByCarCount>> GetBrandWithCountAsync();
    }
}
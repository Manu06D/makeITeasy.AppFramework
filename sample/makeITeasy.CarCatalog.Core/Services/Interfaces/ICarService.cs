using System.Collections.Generic;
using makeITeasy.AppFramework.Core.Interfaces;
using makeITeasy.CarCatalog.Models;
using makeITeasy.CarCatalog.Models.Custom;

namespace makeITeasy.CarCatalog.Core.Services.Interfaces
{
    public interface ICarService : IBaseEntityService<Car>
    {
        List<BrandGroupByCarCount> GetBrandWithCount();
    }
}
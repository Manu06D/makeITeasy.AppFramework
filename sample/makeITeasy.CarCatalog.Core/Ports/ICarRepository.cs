using System.Collections.Generic;
using makeITeasy.AppFramework.Core.Interfaces;
using makeITeasy.CarCatalog.Models;
using makeITeasy.CarCatalog.Models.Custom;

namespace makeITeasy.CarCatalog.Core.Ports
{
    public interface ICarRepository : IAsyncRepository<Car>
    {
        List<BrandGroupByCarCount> OwnRepositoryMethod();
    }
}

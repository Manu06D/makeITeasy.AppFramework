using makeITeasy.AppFramework.Core.Interfaces;
using makeITeasy.CarCatalog.dotnet10.Models;
using makeITeasy.CarCatalog.dotnet10.Models.Custom;


namespace makeITeasy.CarCatalog.dotnet10.Core.Ports
{
    public interface ICarRepository : IAsyncRepository<Car>
    {
        Task<List<BrandGroupByCarCount>> GroupByBrandAndCountAsync();
    }
}

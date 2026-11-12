using makeITeasy.AppFramework.Core.Interfaces;
using makeITeasy.CarCatalog.dotnet11.Models;
using makeITeasy.CarCatalog.dotnet11.Models.Custom;


namespace makeITeasy.CarCatalog.dotnet11.Core.Ports
{
    public interface ICarRepository : IAsyncRepository<Car>
    {
        Task<List<BrandGroupByCarCount>> GroupByBrandAndCountAsync();

        Task<int> AddCarsInResilientTransactionAsync(IEnumerable<Car> cars, CancellationToken cancellationToken = default);
    }
}

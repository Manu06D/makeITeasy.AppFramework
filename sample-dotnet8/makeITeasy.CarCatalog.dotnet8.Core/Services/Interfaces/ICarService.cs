using makeITeasy.AppFramework.Core.Interfaces;
using makeITeasy.CarCatalog.dotnet8.Models;
using makeITeasy.CarCatalog.dotnet8.Models.Custom;

namespace makeITeasy.CarCatalog.dotnet8.Core.Services.Interfaces
{
    public interface ICarService : IBaseEntityService<Car>
    {
        Task<List<BrandGroupByCarCount>> GetBrandWithCountAsync();
    }
}
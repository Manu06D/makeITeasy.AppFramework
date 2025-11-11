using makeITeasy.AppFramework.Core.Interfaces;
using makeITeasy.CarCatalog.dotnet10.Models;
using makeITeasy.CarCatalog.dotnet10.Models.Custom;

using System.Linq.Expressions;

namespace makeITeasy.CarCatalog.dotnet10.Core.Services.Interfaces
{
    public interface ICarService : IBaseEntityService<Car>
    {
        Task<List<BrandGroupByCarCount>> GetBrandWithCountAsync();
    }
}
using System.Collections.Generic;
using System.Threading.Tasks;

using makeITeasy.AppFramework.Core.Interfaces;
using makeITeasy.CarCatalog.dotnet7.Models.Custom;
using makeITeasy.CarCatalog.dotnet7.Models;

namespace makeITeasy.CarCatalog.dotnet7.Core.Services.Interfaces
{
    public interface ICarService : IBaseEntityService<Car>
    {
        Task<List<BrandGroupByCarCount>> GetBrandWithCountAsync();
    }
}

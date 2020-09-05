using makeITeasy.AppFramework.Core.Interfaces;
using makeITeasy.CarCatalog.Models;

namespace makeITeasy.CarCatalog.Core.Services.Interfaces
{
    public interface ICarService : IBaseEntityService<Car>
    {
        bool IsValid(Car car);
    }
}
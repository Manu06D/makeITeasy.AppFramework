using makeITeasy.AppFramework.Core.Interfaces;
using makeITeasy.CarCatalog.Models;

namespace makeITeasy.CarCatalog.Core.Domains.CarDomain
{
    public interface ICarService : IBaseEntityService<Car>
    {
        bool IsValid(Car car);
    }
}
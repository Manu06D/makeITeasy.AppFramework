using makeITeasy.AppFramework.Core.Interfaces;
using makeITeasy.CarCatalog.Models;

namespace makeITeasy.CarCatalog.Core.Ports
{
    public interface ICarRepository : IAsyncRepository<Car>
    {
        void OwnRepositoryMethod();
    }
}

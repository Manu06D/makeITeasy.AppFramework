using AutoMapper;
using makeITeasy.AppFramework.Core.Infrastructure.Persistence;
using makeITeasy.CarCatalog.Core.Ports;
using makeITeasy.CarCatalog.Infrastructure.Data;
using makeITeasy.CarCatalog.Models;

namespace makeITeasy.CarCatalog.Infrastructure.Repositories
{
    public class CarRepository : BaseEfRepository<Car, CarCatalogContext>, ICarRepository
    {
        public CarRepository(CarCatalogContext dbContext, IMapper mapper) : base(dbContext, mapper)
        {
        }

        public void OwnRepositoryMethod()
        {
        }
    }
}

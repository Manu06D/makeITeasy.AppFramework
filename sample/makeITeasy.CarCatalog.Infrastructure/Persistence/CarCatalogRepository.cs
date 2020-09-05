using AutoMapper;
using makeITeasy.AppFramework.Core.Infrastructure.Persistence;
using makeITeasy.AppFramework.Models;
using makeITeasy.CarCatalog.Infrastructure.Data;

namespace makeITeasy.CarCatalog.Infrastructure.Persistence
{
    public class CarCatalogRepository<T> : BaseEfRepository<T, CarCatalogContext> where T : BaseEntity
    {
        public CarCatalogRepository(CarCatalogContext dbContext, IMapper mapper) : base(dbContext, mapper)
        {
        }
    }
}

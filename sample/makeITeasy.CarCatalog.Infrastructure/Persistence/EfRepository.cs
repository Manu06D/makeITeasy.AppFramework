using AutoMapper;
using makeITeasy.AppFramework.Core.Infrastructure.Persistence;
using makeITeasy.AppFramework.Models;
using makeITeasy.CarCatalog.Infrastructure.Data;

namespace makeITeasy.CarCatalog.Infrastructure.Persistence
{
    public class EfRepository<T> : BaseEfRepository<T, CarCatalogContext> where T : BaseEntity
    {
        public EfRepository(CarCatalogContext dbContext, IMapper mapper) : base(dbContext, mapper)
        {
        }
    }
}

using AutoMapper;
using makeITeasy.AppFramework.Core.Infrastructure.Persistence;
using makeITeasy.AppFramework.Models;
using makeITeasy.CarCatalog.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace makeITeasy.CarCatalog.Infrastructure.Persistence
{
    public class CarCatalogRepository<T> : BaseEfRepository<T, CarCatalogContext> where T : class, IBaseEntity
    {
        public CarCatalogRepository(IDbContextFactory<CarCatalogContext> dbFactory, IMapper mapper) : base(dbFactory, mapper)
        {
        }
    }
}

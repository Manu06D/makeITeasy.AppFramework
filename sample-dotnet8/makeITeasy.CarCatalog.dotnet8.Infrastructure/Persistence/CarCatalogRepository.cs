using AutoMapper;

using makeITeasy.AppFramework.Infrastructure.EF8.Persistence;
using makeITeasy.AppFramework.Models;
using makeITeasy.CarCatalog.dotnet8.Infrastructure.Data;

using Microsoft.EntityFrameworkCore;

namespace makeITeasy.CarCatalog.dotnet8.Infrastructure.Persistence
{
    public class CarCatalogRepository<T> : BaseEfRepository<T, CarCatalogContext> where T : class, IBaseEntity
    {
        public CarCatalogRepository(IDbContextFactory<CarCatalogContext> dbFactory, IMapper mapper) : base(dbFactory, mapper)
        {
        }
    }
}

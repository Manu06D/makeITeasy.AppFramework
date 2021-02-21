using AutoMapper;
using makeITeasy.AppFramework.Infrastructure.Persistence;
using makeITeasy.AppFramework.Models;
using makeITeasy.CarCatalog.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace makeITeasy.CarCatalog.Infrastructure.Persistence
{
    public class TransactionCarCatalogRepository<T> : TransactionEfRepository<T, CarCatalogContext> where T : class, IBaseEntity
    {
        public TransactionCarCatalogRepository(IDbContextFactory<CarCatalogContext> dbFactory, IMapper mapper) : base(dbFactory, mapper)
        {
        }
    }
}

using AutoMapper;

using makeITeasy.AppFramework.Infrastructure.EF7.Persistence;
using makeITeasy.AppFramework.Models;
using makeITeasy.CarCatalog.dotnet7.Infrastructure.Data;

using Microsoft.EntityFrameworkCore;

namespace makeITeasy.CarCatalog.dotnet7.Infrastructure.Persistence
{
    public class TransactionCarCatalogRepository<T> : TransactionEfRepository<T, CarCatalogContext> where T : class, IBaseEntity
    {
        public TransactionCarCatalogRepository(IDbContextFactory<CarCatalogContext> dbFactory, IMapper mapper) : base(dbFactory, mapper)
        {
        }
    }
}

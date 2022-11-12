using AutoMapper;

using makeITeasy.AppFramework.Infrastructure.EF7.Persistence;
using makeITeasy.AppFramework.Models;

using Microsoft.EntityFrameworkCore;

namespace makeITeasy.CarCatalog.dotnet7.Infrastructure.Persistence
{
    public class EfRepository<T> : BaseEfRepository<T, DbContext> where T : class, IBaseEntity
    {
        public EfRepository(IDbContextFactory<DbContext> dbFactory, IMapper mapper) : base(dbFactory, mapper)
        {
        }

        public EfRepository(DbContext dbContext, IMapper mapper) : base(dbContext, mapper)
        {

        }
    }
}

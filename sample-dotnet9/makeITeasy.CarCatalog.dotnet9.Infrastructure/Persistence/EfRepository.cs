using makeITeasy.AppFramework.Infrastructure.EntityFramework.Persistence;
using makeITeasy.AppFramework.Models;

using Microsoft.EntityFrameworkCore;

namespace makeITeasy.CarCatalog.dotnet9.Infrastructure.Persistence
{
    public class EfRepository<T> : BaseEfRepository<T, DbContext> where T : class, IBaseEntity
    {
        public EfRepository(IDbContextFactory<DbContext> dbFactory) : base(dbFactory)
        {
        }

        public EfRepository(DbContext dbContext) : base(dbContext)
        {

        }
    }
}

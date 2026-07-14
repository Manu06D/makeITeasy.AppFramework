using EFCore.BulkExtensions;

using makeITeasy.AppFramework.Core.Interfaces;
using makeITeasy.AppFramework.Models;

using Microsoft.EntityFrameworkCore;

using System.Linq.Expressions;

namespace makeITeasy.AppFramework.Infrastructure.EntityFramework.Persistence
{
    public partial class BaseEfRepository<T, U> : IAsyncRepository<T> where T : class, IBaseEntity where U : DbContext
    {
        public async Task<int> UpdateRangeAsync(Expression<Func<T, bool>> entityPredicate, Expression<Func<T, T>> updateExpression)
        {
            return await GetDbContext().Set<T>().Where(entityPredicate).BatchUpdateAsync(updateExpression);
        }
    }
}

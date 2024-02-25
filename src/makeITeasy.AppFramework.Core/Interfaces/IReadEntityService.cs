using makeITeasy.AppFramework.Core.Queries;
using makeITeasy.AppFramework.Models;

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace makeITeasy.AppFramework.Core.Interfaces
{
    public interface IReadEntityService<TEntity> where TEntity : class, IBaseEntity
    {
        Task<TEntity?> GetByIdAsync(object id, List<Expression<Func<TEntity, object>>>? includes = null, CancellationToken cancellationToken = default);
        Task<IList<TEntity>> ListAllAsync(List<Expression<Func<TEntity, object>>>? includes = null, CancellationToken cancellationToken = default);
        Task<QueryResult<TEntity>> QueryAsync(ISpecification<TEntity> specification, bool includeCount = false, CancellationToken cancellationToken = default);
        Task<TEntity> GetFirstByQueryAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default);
        Task<QueryResult<TTarget>> QueryWithProjectionAsync<TTarget>(ISpecification<TEntity> specification, bool includeCount = false, CancellationToken cancellationToken = default) where TTarget : class;
        Task<TTarget> GetFirstByQueryWithProjectionAsync<TTarget>(ISpecification<TEntity> specification, CancellationToken cancellationToken = default) where TTarget : class;
    }
}

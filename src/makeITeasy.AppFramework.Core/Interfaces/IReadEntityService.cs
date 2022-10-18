using makeITeasy.AppFramework.Core.Queries;
using makeITeasy.AppFramework.Models;

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace makeITeasy.AppFramework.Core.Interfaces
{
    public interface IReadEntityService<TEntity> where TEntity : class, IBaseEntity
    {
        Task<TEntity?> GetByIdAsync(object id, List<Expression<Func<TEntity, object>>>? includes = null);
        Task<IList<TEntity>> ListAllAsync(List<Expression<Func<TEntity, object>>>? includes = null);
        Task<QueryResult<TEntity>> QueryAsync(ISpecification<TEntity> specification, bool includeCount = false);
        Task<TEntity> GetFirstByQueryAsync(ISpecification<TEntity> specification);
        Task<QueryResult<TTarget>> QueryWithProjectionAsync<TTarget>(ISpecification<TEntity> specification, bool includeCount = false) where TTarget : class;
        Task<TTarget> GetFirstByQueryWithProjectionAsync<TTarget>(ISpecification<TEntity> specification) where TTarget : class;
    }
}

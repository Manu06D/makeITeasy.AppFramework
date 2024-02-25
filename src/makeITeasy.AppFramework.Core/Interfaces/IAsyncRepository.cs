using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using makeITeasy.AppFramework.Models;
using makeITeasy.AppFramework.Core.Queries;
using makeITeasy.AppFramework.Core.Commands;
using System.Threading;

namespace makeITeasy.AppFramework.Core.Interfaces
{
    public interface IAsyncRepository<T> : IDisposable where T : class, IBaseEntity
    {
        Task<T?> GetByIdAsync(object id, CancellationToken cancellationToken = default);
        Task<T?> GetByIdAsync(object id, List<Expression<Func<T, object>>>? includes, CancellationToken cancellationToken = default);
        Task<IList<T>> ListAllAsync(List<Expression<Func<T, object>>>? includes = null, CancellationToken cancellationToken = default);
        Task<QueryResult<T>> ListAsync(ISpecification<T> spec, bool includeCount = false, CancellationToken cancellationToken = default);
        Task<QueryResult<TTarget>> ListWithProjectionAsync<TTarget>(ISpecification<T> spec, bool includeCount = false, CancellationToken cancellationToken = default) where TTarget : class;
        Task<T> AddAsync(T entity, bool saveChanges = true, CancellationToken cancellationToken = default);
        Task<CommandResult<T>> UpdateAsync(T entity, bool saveChanges = true, CancellationToken cancellationToken = default);
        Task<CommandResult<T>> UpdatePropertiesAsync(T entity, string[] propertyNames, bool saveChanges = true, CancellationToken cancellationToken = default);
        Task<CommandResult> DeleteAsync(T entity, bool saveChanges = true, CancellationToken cancellationToken = default);
        Task<int> CountAsync(ISpecification<T> spec, CancellationToken cancellationToken = default);
        Task<ICollection<T>> AddRangeAsync(ICollection<T> entities, bool saveChanges = true, CancellationToken cancellationToken = default);
        Task<int> UpdateRangeAsync(Expression<Func<T, bool>> entityPredicate, Expression<Func<T, T>> updateExpression, CancellationToken cancellationToken = default);
    }
}

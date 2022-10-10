using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using makeITeasy.AppFramework.Models;
using makeITeasy.AppFramework.Core.Queries;
using makeITeasy.AppFramework.Core.Commands;

namespace makeITeasy.AppFramework.Core.Interfaces
{
    public interface IAsyncRepository<T> : IDisposable where T : class, IBaseEntity
    {
        Task<T?> GetByIdAsync(object id);
        Task<T?> GetByIdAsync(object id, List<Expression<Func<T, object>>>? includes);
        Task<IList<T>> ListAllAsync(List<Expression<Func<T, object>>>? includes = null);
        Task<QueryResult<T>> ListAsync(ISpecification<T> spec, bool includeCount = false);
        Task<QueryResult<TTarget>> ListWithProjectionAsync<TTarget>(ISpecification<T> spec, bool includeCount = false) where TTarget : class;
        Task<T> AddAsync(T entity, bool saveChanges = true);
        Task<CommandResult<T>> UpdateAsync(T entity, bool saveChanges = true);
        Task<CommandResult<T>> UpdatePropertiesAsync(T entity, string[] propertyNames, bool saveChanges = true);
        Task<CommandResult> DeleteAsync(T entity, bool saveChanges = true);
        Task<int> CountAsync(ISpecification<T> spec);
        Task<ICollection<T>> AddRangeAsync(ICollection<T> entities, bool saveChanges = true);
        Task<int> UpdateRangeAsync(Expression<Func<T, bool>> entityPredicate, Expression<Func<T, T>> updateExpression);
    }
}

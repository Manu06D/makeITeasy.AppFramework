using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using makeITeasy.AppFramework.Models;
using makeITeasy.AppFramework.Core.Queries;
using makeITeasy.AppFramework.Core.Commands;

namespace makeITeasy.AppFramework.Core.Interfaces
{
    public interface IBaseEntityService<T> where T : class, IBaseEntity
    {
        Task<T> GetByIdAsync(object id, List<Expression<Func<T, object>>> includes = null);
        Task<QueryResult<T>> QueryAsync(BaseQuery<T> specification, bool includeCount = false);
        Task<QueryResult<TTarget>> QueryWithProjectionAsync<TTarget>(BaseQuery<T> specification, bool includeCount = false) where TTarget : class;
        Task<CommandResult<T>> CreateAsync(T entity, bool saveChanges = true);
        Task<CommandResult<T>> UpdateAsync(T entity);
        Task<CommandResult<T>> UpdatePropertiesAsync(T entity, string[] properties);
        Task<CommandResult> DeleteAsync(T entity, bool saveChanges = true);
        bool Validate(T entity);
    }
}

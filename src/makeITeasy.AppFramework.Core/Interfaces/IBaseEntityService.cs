using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using makeITeasy.AppFramework.Models;
using makeITeasy.AppFramework.Core.Queries;
using makeITeasy.AppFramework.Core.Commands;

namespace makeITeasy.AppFramework.Core.Interfaces
{
    public interface IBaseEntityService<TEntity> where TEntity : class, IBaseEntity
    {
        Task<TEntity> GetByIdAsync(object id, List<Expression<Func<TEntity, object>>> includes = null);
        Task<QueryResult<TEntity>> QueryAsync(ISpecification<TEntity> specification, bool includeCount = false);
        Task<QueryResult<TTarget>> QueryWithProjectionAsync<TTarget>(ISpecification<TEntity> specification, bool includeCount = false) where TTarget : class;
        Task<CommandResult<TEntity>> CreateAsync(TEntity entity, bool saveChanges = true);
        Task<CommandResult<TEntity>> UpdateAsync(TEntity entity);
        Task<CommandResult<TEntity>> UpdatePropertiesAsync(TEntity entity, string[] properties);
        Task<CommandResult> DeleteAsync(TEntity entity, bool saveChanges = true);
        bool Validate(TEntity entity);
        Task<ICollection<CommandResult<TEntity>>> CreateRangeAsync(ICollection<TEntity> entities, bool saveChanges = true);

        Task<int> UpdateRangeAsync(Expression<Func<TEntity, bool>> entityPredicate, Expression<Func<TEntity, TEntity>> updateExpression);
    }
}

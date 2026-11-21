using System.Linq.Expressions;
using makeITeasy.AppFramework.Core.Commands;
using makeITeasy.AppFramework.Core.Models;

namespace makeITeasy.AppFramework.Core.Interfaces
{
    public interface IBaseEntityService<TEntity> : IReadEntityService<TEntity> where TEntity : class, IBaseEntity
    {
        Task<CommandResult<TEntity>> CreateAsync(TEntity entity, bool saveChanges = true, CancellationToken cancellationToken = default);
        Task<CommandResult<TEntity>> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);
        Task<CommandResult<TEntity>> UpdatePropertiesAsync(TEntity entity, string[] properties, bool forceLastModificationDateUpdate = false, CancellationToken cancellationToken = default);
        Task<CommandResult> DeleteAsync(TEntity entity, bool saveChanges = true, CancellationToken cancellationToken = default);
        Task<ICollection<CommandResult<TEntity>>> CreateRangeAsync(ICollection<TEntity> entities, bool saveChanges = true, CancellationToken cancellationToken = default);
        Task<int> UpdateRangeAsync(Expression<Func<TEntity, bool>> entityPredicate, Expression<Func<TEntity, TEntity>> updateExpression, CancellationToken cancellationToken = default);
        Task<int> UpdateRangeAsync(Expression<Func<TEntity, bool>> entityPredicate, UpdateDefinition<TEntity> updates, CancellationToken cancellationToken = default);
        bool Validate(TEntity entity);
    }
}

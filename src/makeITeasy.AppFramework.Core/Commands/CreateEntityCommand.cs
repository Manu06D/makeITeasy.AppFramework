using makeITeasy.AppFramework.Models;
using MediatR;

namespace makeITeasy.AppFramework.Core.Commands
{
    public class CreateEntityCommand<TEntity> : ICommandEntity<TEntity>, IRequest<CommandResult<TEntity>> where TEntity : IBaseEntity
    {
        public TEntity Entity { get; private set; }

        public CreateEntityCommand(TEntity entity)
        {
            this.Entity = entity;
        }
    }
}

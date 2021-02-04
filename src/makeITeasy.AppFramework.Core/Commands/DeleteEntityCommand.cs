using makeITeasy.AppFramework.Models;
using MediatR;

namespace makeITeasy.AppFramework.Core.Commands
{
    public class DeleteEntityCommand<TEntity> : ICommandEntity<TEntity>, IRequest<CommandResult> where TEntity : IBaseEntity
    {
        public TEntity Entity { get; private set; }

        public DeleteEntityCommand(TEntity entity)
        {
            this.Entity = entity;
        }
    }
}

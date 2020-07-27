using makeITeasy.AppFramework.Models;
using MediatR;

namespace makeITeasy.AppFramework.Core.Commands
{
    public class UpdateEntityCommand<TEntity> : IRequest<CommandResult<TEntity>> where TEntity : BaseEntity
    {
        public TEntity Entity { get; private set; }


        public UpdateEntityCommand(TEntity entity)
        {
            this.Entity = entity;
        }
    }
}

using makeITeasy.AppFramework.Models;
using MediatR;

namespace makeITeasy.AppFramework.Core.Commands
{
    public class UpdatePartialEntityCommand<TEntity> : IRequest<CommandResult<TEntity>> where TEntity : IBaseEntity
    {
        public TEntity Entity { get; private set; }


        public string[] Properties { get; private set; }

        public UpdatePartialEntityCommand(TEntity entity, string[] properties)
        {
            Entity = entity;
            Properties = properties;
        }
    }
}

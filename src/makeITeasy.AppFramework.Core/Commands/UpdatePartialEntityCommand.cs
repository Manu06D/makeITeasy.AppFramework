using MediatR;

namespace makeITeasy.AppFramework.Core.Commands
{
    public class UpdatePartialEntityCommand<TEntity> : ICommandEntity<TEntity>, IRequest<CommandResult<TEntity>> where TEntity : IBaseEntity
    {
        public TEntity Entity { get; private set; }

        public string[] Properties { get; private set; }

        public bool ForceLastModificationDateUpdate { get; private set; }

        public UpdatePartialEntityCommand(TEntity entity, string[] properties, bool forceLastModificationDateUpdate = false)
        {
            Entity = entity;
            Properties = properties;
            ForceLastModificationDateUpdate = forceLastModificationDateUpdate;
        }
    }
}

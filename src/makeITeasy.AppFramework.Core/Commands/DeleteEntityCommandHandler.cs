using System.Threading;
using System.Threading.Tasks;
using makeITeasy.AppFramework.Core.Interfaces;
using makeITeasy.AppFramework.Models;
using MediatR;

namespace makeITeasy.AppFramework.Core.Commands
{
    public class DeleteEntityCommandHandler<TEntity> : IRequestHandler<DeleteEntityCommand<TEntity>, CommandResult<TEntity>> where TEntity : class, IBaseEntity
    {
        private readonly IBaseEntityService<TEntity> baseService;

        public DeleteEntityCommandHandler(IBaseEntityService<TEntity> baseService)
        {
            this.baseService = baseService;
        }
        public async Task<CommandResult<TEntity>> Handle(DeleteEntityCommand<TEntity> request, CancellationToken cancellationToken)
        {
            int nb = await baseService.DeleteAsync(request.Entity);
            if (nb > 0)
            {
                return new CommandResult<TEntity>() { Entity = request.Entity, Message = $"{nb} {(nb == 1 ? "entity" : "entities")} deleted.", Result = CommandState.Success };
            }
            else
            {
                return new CommandResult<TEntity>() { Entity = request.Entity, Message = "no entity deleted.", Result = CommandState.Warning };
            }
        }
    }
}

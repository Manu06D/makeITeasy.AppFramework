using System.Threading;
using System.Threading.Tasks;
using makeITeasy.AppFramework.Core.Interfaces;
using makeITeasy.AppFramework.Models;
using MediatR;

namespace makeITeasy.AppFramework.Core.Commands
{
    public class DeleteEntityCommandHandler<TEntity> : IRequestHandler<DeleteEntityCommand<TEntity>, CommandResult> where TEntity : class, IBaseEntity
    {
        private readonly IBaseEntityService<TEntity> baseService;

        public DeleteEntityCommandHandler(IBaseEntityService<TEntity> baseService)
        {
            this.baseService = baseService;
        }
        public async Task<CommandResult> Handle(DeleteEntityCommand<TEntity> request, CancellationToken cancellationToken)
        {
            CommandResult result = await baseService.DeleteAsync(request.Entity);

            return result;           
        }
    }
}

using System.Threading;
using System.Threading.Tasks;
using makeITeasy.AppFramework.Core.Interfaces;
using makeITeasy.AppFramework.Models;
using MediatR;

namespace makeITeasy.AppFramework.Core.Commands
{
    public class CreateEntityCommandHandler<TEntity> : IRequestHandler<UpdateEntityCommand<TEntity>, CommandResult<TEntity>> where TEntity : BaseEntity
    {
        private readonly IBaseEntityService<TEntity> baseService;

        public CreateEntityCommandHandler(IBaseEntityService<TEntity> baseService)
        {
            this.baseService = baseService;
        }
        public async Task<CommandResult<TEntity>> Handle(UpdateEntityCommand<TEntity> request, CancellationToken cancellationToken)
        {
            return await baseService.UpdateAsync(request?.Entity);
        }
    }
}

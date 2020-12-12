using System.Threading;
using System.Threading.Tasks;
using makeITeasy.AppFramework.Core.Interfaces;
using makeITeasy.AppFramework.Models;
using MediatR;


namespace makeITeasy.AppFramework.Core.Commands
{
    public class UpdatePartialEntityCommandHandler<TEntity> : IRequestHandler<UpdatePartialEntityCommand<TEntity>, CommandResult<TEntity>> where TEntity : class, IBaseEntity
    {
        private readonly IBaseEntityService<TEntity> baseService;

        public UpdatePartialEntityCommandHandler(IBaseEntityService<TEntity> baseService)
        {
            this.baseService = baseService;
        }

        public async Task<CommandResult<TEntity>> Handle(UpdatePartialEntityCommand<TEntity> request, CancellationToken cancellationToken)
        {
            return await baseService.UpdatePropertiesAsync(request?.Entity, request?.Properties);
        }
    }
}

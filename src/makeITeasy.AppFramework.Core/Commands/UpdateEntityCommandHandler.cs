using System;
using System.Threading;
using System.Threading.Tasks;
using makeITeasy.AppFramework.Core.Interfaces;
using makeITeasy.AppFramework.Models;
using MediatR;

namespace makeITeasy.AppFramework.Core.Commands
{
    public class UpdateEntityCommandHandler<TEntity> : IRequestHandler<UpdateEntityCommand<TEntity>, CommandResult<TEntity>> where TEntity : class, IBaseEntity
    {
        private readonly IBaseEntityService<TEntity> baseService;

        public UpdateEntityCommandHandler(IBaseEntityService<TEntity> baseService)
        {
            this.baseService = baseService;
        }
        public async Task<CommandResult<TEntity>> Handle(UpdateEntityCommand<TEntity> request, CancellationToken cancellationToken)
        {
            if (request is null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            return await baseService.UpdateAsync(request.Entity);
        }
    }
}

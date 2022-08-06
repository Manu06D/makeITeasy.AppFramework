using makeITeasy.AppFramework.Core.Interfaces;
using makeITeasy.AppFramework.Models;
using MediatR;

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace makeITeasy.AppFramework.Core.Queries
{
    public class GenericFindUniqueCommandHandler<TEntity> : IRequestHandler<GenericFindUniqueCommand<TEntity>, TEntity> where TEntity : class, IBaseEntity
    {
        private readonly IBaseEntityService<TEntity> baseService;

        public GenericFindUniqueCommandHandler(IBaseEntityService<TEntity> baseService)
        {
            this.baseService = baseService;
        }

        public async Task<TEntity> Handle(GenericFindUniqueCommand<TEntity> request, CancellationToken cancellationToken)
        {
            return await baseService.GetFirstByQueryAsync(request?.Query);
        }
    }
}
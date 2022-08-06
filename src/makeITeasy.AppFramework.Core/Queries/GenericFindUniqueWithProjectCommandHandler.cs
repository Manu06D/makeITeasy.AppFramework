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
    public class GenericFindUniqueWithProjectCommandHandler<TEntity, TResult> : IRequestHandler<GenericFindUniqueWithProjectCommand<TEntity, TResult>, TResult> where TEntity : class, IBaseEntity where TResult : class
    {
        private readonly IBaseEntityService<TEntity> baseService;

        public GenericFindUniqueWithProjectCommandHandler(IBaseEntityService<TEntity> baseService)
        {
            this.baseService = baseService;
        }

        public async Task<TResult> Handle(GenericFindUniqueWithProjectCommand<TEntity, TResult> request, CancellationToken cancellationToken)
        {
            return await baseService.GetFirstByQueryWithProjectionAsync<TResult>(request?.Query);
        }
    }
}
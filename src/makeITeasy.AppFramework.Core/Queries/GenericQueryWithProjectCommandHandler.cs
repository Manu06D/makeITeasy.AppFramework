using System.Threading;
using System.Threading.Tasks;
using makeITeasy.AppFramework.Core.Interfaces;
using makeITeasy.AppFramework.Models;
using MediatR;

namespace makeITeasy.AppFramework.Core.Queries
{
    public class GenericQueryWithProjectCommandHandler<TEntity, TResult> : IRequestHandler<GenericQueryWithProjectCommand<TEntity, TResult>, QueryResult<TResult>> where TEntity : class, IBaseEntity where TResult : class
    {
        private readonly IBaseEntityService<TEntity> baseService;

        public GenericQueryWithProjectCommandHandler(IBaseEntityService<TEntity> baseService)
        {
            this.baseService = baseService;
        }

        public async Task<QueryResult<TResult>> Handle(GenericQueryWithProjectCommand<TEntity, TResult> request, CancellationToken cancellationToken)
        {
            return await baseService.QueryWithProjectionAsync<TResult>(request?.Query, request?.IncludeCount == true);
        }
    }
}

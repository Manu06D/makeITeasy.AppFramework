using System.Threading;
using System.Threading.Tasks;
using makeITeasy.AppFramework.Core.Interfaces;
using makeITeasy.AppFramework.Models;
using MediatR;

namespace makeITeasy.AppFramework.Core.Queries
{
    public class GenericQueryWithProjectCommandHandler<T, TResult> : IRequestHandler<GenericQueryWithProjectCommand<T, TResult>, QueryResult<TResult>> where T : BaseEntity where TResult : class
    {
        private readonly IBaseEntityService<T> baseService;

        public GenericQueryWithProjectCommandHandler(IBaseEntityService<T> baseService)
        {
            this.baseService = baseService;
        }

        public async Task<QueryResult<T>> Handle(GenericQueryCommand<T> request, CancellationToken cancellationToken)
        {
            return await baseService.QueryAsync(request?.Query, request.IncludeCount);
        }

        public async Task<QueryResult<TResult>> Handle(GenericQueryWithProjectCommand<T, TResult> request, CancellationToken cancellationToken)
        {
            return await baseService.QueryWithProjectionAsync<TResult>(request?.Query, request.IncludeCount);
        }
    }
}

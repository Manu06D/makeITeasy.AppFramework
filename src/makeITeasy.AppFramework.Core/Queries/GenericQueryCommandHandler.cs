using System.Threading;
using System.Threading.Tasks;
using makeITeasy.AppFramework.Core.Interfaces;
using makeITeasy.AppFramework.Models;
using MediatR;

namespace makeITeasy.AppFramework.Core.Queries
{
    public class GenericQueryCommandHandler<TEntity> : IRequestHandler<GenericQueryCommand<TEntity>, QueryResult<TEntity>> where TEntity : BaseEntity
    {
        private readonly IBaseEntityService<TEntity> baseService;

        public GenericQueryCommandHandler(IBaseEntityService<TEntity> baseService)
        {
            this.baseService = baseService;
        }

        public async Task<QueryResult<TEntity>> Handle(GenericQueryCommand<TEntity> request, CancellationToken cancellationToken)
        {
            return await baseService.QueryAsync(request?.Query, request?.IncludeCount ?? false);
        }
    }
}
    
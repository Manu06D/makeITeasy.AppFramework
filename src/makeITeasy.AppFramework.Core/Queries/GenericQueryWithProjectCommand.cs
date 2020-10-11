using makeITeasy.AppFramework.Models;
using MediatR;

namespace makeITeasy.AppFramework.Core.Queries
{
    public class GenericQueryWithProjectCommand<TRequest, TResult> : IRequest<QueryResult<TResult>> where TRequest : class, IBaseEntity where TResult : class
    {
        public BaseQuery<TRequest> Query { get; }
        public bool IncludeCount { get; }

        public GenericQueryWithProjectCommand(BaseQuery<TRequest> query, bool includeCount = false)
        {
            this.Query = query;
            this.IncludeCount = includeCount;
        }
    }
}

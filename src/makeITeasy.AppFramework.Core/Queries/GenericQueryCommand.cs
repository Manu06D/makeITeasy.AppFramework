using makeITeasy.AppFramework.Models;
using MediatR;

namespace makeITeasy.AppFramework.Core.Queries
{
    public class GenericQueryCommand<TRequest> : IRequest<QueryResult<TRequest>> where TRequest : BaseEntity
    {
        public BaseQuery<TRequest> Query { get; }
        public bool IncludeCount { get; }

        public GenericQueryCommand(BaseQuery<TRequest> query, bool includeCount = false)
        {
            this.Query = query;
            this.IncludeCount = includeCount;
        }
    }
}

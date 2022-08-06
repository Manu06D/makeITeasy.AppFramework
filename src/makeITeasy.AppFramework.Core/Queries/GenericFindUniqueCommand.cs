using makeITeasy.AppFramework.Models;
using MediatR;

using System;
using System.Collections.Generic;
using System.Text;

namespace makeITeasy.AppFramework.Core.Queries
{
    public class GenericFindUniqueCommand<TRequest> : IRequest<TRequest> where TRequest : class, IBaseEntity
    {
        public BaseQuery<TRequest> Query { get; }

        public GenericFindUniqueCommand(BaseQuery<TRequest> query)
        {
            Query = query;
        }
    }
}

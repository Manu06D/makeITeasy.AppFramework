using makeITeasy.AppFramework.Models;
using MediatR;

using System;
using System.Collections.Generic;
using System.Text;

namespace makeITeasy.AppFramework.Core.Queries
{
    public class GenericFindUniqueWithProjectCommand<TRequest, TResult> : IRequest<TResult> where TRequest : class, IBaseEntity where TResult : class
    {
        public ISpecification<TRequest> Query { get; }

        public GenericFindUniqueWithProjectCommand(ISpecification<TRequest> query)
        {
            Query = query;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace makeITeasy.AppFramework.Models
{
    public interface ISpecification<T>
    {
        Expression<Func<T, bool>> Criteria { get; }
        List<Expression<Func<T, object>>> Includes { get; }
        List<string> IncludeStrings { get; }
        string SortBy { get; set; }
        bool SortDescending { get; set; }
        int? Take { get; }
        int? Skip { get; }
        bool IsPagingEnabled { get; }
    }
}

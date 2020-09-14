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
        List<OrderBySpecification<string>> OrderByStrings { get; set; }
        List<OrderBySpecification<Expression<Func<T, object>>>> OrderBy { get; set; }
        int? Take { get; set; }
        int? Skip { get; set; }
        bool IsPagingEnabled { get; set; }
        void AddOrder(string orderByColumn, bool sortDescending = false);
        void AddOrder(Expression<Func<T, object>> orderByColumn, bool sortDescending = false);
        void AddFunctionToCriteria(Expression<Func<T, bool>> funcToAdd, FunctionAggregatorEnum type = FunctionAggregatorEnum.And);
    }
}

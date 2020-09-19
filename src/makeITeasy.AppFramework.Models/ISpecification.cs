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
        bool IsPagingEnabled { get; }
        void AddOrder(OrderBySpecification<string> spec);
        void AddOrder(OrderBySpecification<Expression<Func<T, object>>> spec);
        void AddFunctionToCriteria(Expression<Func<T, bool>> funcToAdd, FunctionAggregatorEnum type = FunctionAggregatorEnum.And);
        void AddInclude(Expression<Func<T, object>> spec);
        void AddInclude(string spec);
    }
}

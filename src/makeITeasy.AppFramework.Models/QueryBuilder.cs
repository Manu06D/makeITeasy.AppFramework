using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace makeITeasy.AppFramework.Models
{
    public interface ICanAddPostBuilder<T>
    {
        ICanAddFunctionFilter Where(Expression<Func<T, bool>> funcToAdd);
    }

    public interface ICanAddFunctionFilter
    {

    }

    public class QueryBuilder
    {
        public static ICanAddPostBuilder<C> Create<C>(ISpecification<C> spec) where C:IBaseEntity
        {
            return new FluentQueryBuilder<C>(spec);
        }
    }

    public class FluentQueryBuilder<T> : ICanAddPostBuilder<T>, ICanAddFunctionFilter  where T:IBaseEntity
    {
        private readonly ISpecification<T> _baseQuery;

        public FluentQueryBuilder(ISpecification<T> basequery)
        {
            _baseQuery = basequery;
        }

        public ICanAddFunctionFilter Where(Expression<Func<T, bool>> funcToAdd)
        {
            _baseQuery.AddFunctionToCriteria(funcToAdd);

            return this;
        }
    }
}

using System;
using System.Linq.Expressions;

namespace makeITeasy.AppFramework.Models
{
    public interface ICanBuild<T> where T:IBaseEntity
    {
        BaseQuery<T> Build();
    }

    public interface ICanAddPostBuilder<T> : ICanBuild<T> where T : IBaseEntity
    {
        ICanAddFunctionFilter<T> Where(Expression<Func<T, bool>> funcToAdd);
        ICanAddThenOrderByOrTake<T> OrderBy(string spec, bool sortDescending);
        ICanAddInclude<T> Include();
    }

    public interface ICanAddThenOrderByOrTake<T> : ICanBuild<T> where T : IBaseEntity
    {
        ICanAddThenOrderByOrTake<T> ThenOrderBy(OrderBySpecification<String> spec);
        ICanSkip<T> Take(int page);
    }


    public interface ICanSkip<T> : ICanBuild<T> where T : IBaseEntity
    {
        ICanAddInclude<T> Skip(int pageSize);
    }


    public interface ICanAddInclude<T> : ICanBuild<T> where T : IBaseEntity
    { 
    }

    public interface ICanAddFunctionFilter<T> : ICanBuild<T> where T : IBaseEntity
    {
        ICanAddThenOrderByOrTake<T> OrderBy(Expression<Func<T, object>> spec, bool sortDescending);
        ICanAddThenOrderByOrTake<T> OrderBy(string spec, bool sortDescending);
        ICanAddInclude<T> Include();
    }

    public class QueryBuilder
    {
        public static ICanAddPostBuilder<C> Create<C>(ISpecification<C> spec) where C:IBaseEntity
        {
            return new FluentQueryBuilder<C>(spec);
        }
    }

    public class FluentQueryBuilder<T> : ICanAddPostBuilder<T>, ICanAddFunctionFilter<T>, ICanAddThenOrderByOrTake<T>, ICanSkip<T>, ICanAddInclude<T> where T:IBaseEntity
    {
        private readonly ISpecification<T> _baseQuery;

        public FluentQueryBuilder(ISpecification<T> basequery)
        {
            _baseQuery = basequery;
        }

        public ICanAddFunctionFilter<T> Where(Expression<Func<T, bool>> funcToAdd)
        {
            _baseQuery.AddFunctionToCriteria(funcToAdd);

            return this;
        }

        public ICanAddInclude<T> Include()
        {
            throw new NotImplementedException();
        }

        public ICanAddThenOrderByOrTake<T> OrderBy(string spec, bool sortDescending)
        {
            _baseQuery.AddOrder(new OrderBySpecification<String>(spec, sortDescending));

            return this;
        }

        public ICanAddThenOrderByOrTake<T> OrderBy(Expression<Func<T, object>> spec, bool sortDescending)
        {
            _baseQuery.AddOrder(new OrderBySpecification<Expression<Func<T, object>>>(spec, sortDescending));
            return this;
        }

        public ICanAddThenOrderByOrTake<T> ThenOrderBy(OrderBySpecification<String> spec)
        {
            _baseQuery.AddOrder(spec);

            return this;
        }

        public ICanSkip<T> Take(int page)
        {
            _baseQuery.Take = page;

            return this;
        }

        public ICanAddInclude<T> Skip(int pageSize)
        {
            _baseQuery.Skip = pageSize;

            return this;
        }

        public BaseQuery<T> Build()
        {
            return _baseQuery as BaseQuery<T>;
        }
    }
}

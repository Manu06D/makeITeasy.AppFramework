using System.Linq.Expressions;

namespace makeITeasy.AppFramework.Models
{
    public interface ICanBuild<T> where T:IBaseEntity
    {
        BaseQuery<T> Build();
        ICanBuild<T> WithSplitQuery(bool useSplitQuery = true);
        ICanBuild<T> UseSplitQuery(bool useSplitQuery = true);
        ICanBuild<T> TagWith(string tag);
        ICanBuild<T> WithTag(string tag);
    }

    public interface ICanAddPostBuilder<T> : ICanOrder<T>, ICanAddInclude<T> where T : IBaseEntity
    {
        ICanAddFunctionFilter<T> Where(Expression<Func<T, bool>> funcToAdd);
        new ICanAddPostBuilder<T> WithSplitQuery(bool useSplitQuery = true);
        new ICanAddPostBuilder<T> UseSplitQuery(bool useSplitQuery = true);
        new ICanAddPostBuilder<T> TagWith(string tag);
        new ICanAddPostBuilder<T> WithTag(string tag);
    }

    public interface ICanAddFunctionFilter<T> : ICanOrder<T> , ICanAddInclude<T> where T : IBaseEntity
    {
        ICanAddFunctionFilter<T> And(Expression<Func<T, bool>> funcToAdd);
        ICanAddFunctionFilter<T> Or(Expression<Func<T, bool>> funcToAdd);
    }

    public interface ICanOrder<T> where T : IBaseEntity
    {
        ICanAddThenOrderByOrTake<T> OrderBy(string spec, bool sortDescending = false);
        ICanAddThenOrderByOrTake<T> OrderBy(Expression<Func<T, object>> spec, bool sortDescending = false);
    }

    public interface ICanAddThenOrderByOrTake<T> : ICanBuild<T>, ICanAddInclude<T> where T : IBaseEntity
    {
        ICanAddThenOrderByOrTake<T> ThenOrderBy(string spec, bool sortDescending = false);
        ICanAddThenOrderByOrTake<T> ThenOrderBy(Expression<Func<T, object>> spec, bool sortDescending = false);

        ICanSkip<T> Take(int page);
    }

    public interface ICanSkip<T> : ICanAddInclude<T> where T : IBaseEntity
    {
        ICanAddInclude<T> Skip(int pageSize);
    }

    public interface ICanAddInclude<T> : ICanBuild<T> where T : IBaseEntity
    {
        ICanAddInclude<T> Include(Expression<Func<T, object>> spec);
        ICanAddInclude<T> Include(string spec);
        new ICanAddInclude<T> WithSplitQuery(bool useSplitQuery = true);
        new ICanAddInclude<T> UseSplitQuery(bool useSplitQuery = true);
        new ICanAddInclude<T> TagWith(string tag);
        new ICanAddInclude<T> WithTag(string tag);
    }

    public class QueryBuilder
    {
        protected QueryBuilder()
        {
        }

        public static ICanAddPostBuilder<C> Create<C>(ISpecification<C> spec) where C:IBaseEntity
        {
            return new FluentQueryBuilder<C>(spec);
        }

        public static ICanAddPostBuilder<C> Create<B,C>() where B : BaseQuery<C>, new() where C:IBaseEntity
        {

            return new FluentQueryBuilder<C>(new B());
        }
    }

    public class FluentQueryBuilder<T> : ICanAddPostBuilder<T>, ICanAddFunctionFilter<T>, ICanAddThenOrderByOrTake<T>, ICanSkip<T> where T:IBaseEntity
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

        public ICanAddThenOrderByOrTake<T> OrderBy(string spec, bool sortDescending = false)
        {
            _baseQuery.AddOrder(new OrderBySpecification<String>(spec, sortDescending));

            return this;
        }

        public ICanAddThenOrderByOrTake<T> OrderBy(Expression<Func<T, object>> spec, bool sortDescending = false)
        {
            _baseQuery.AddOrder(new OrderBySpecification<Expression<Func<T, object>>>(spec, sortDescending));
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

        public ICanAddFunctionFilter<T> And(Expression<Func<T, bool>> funcToAdd)
        {
            _baseQuery.AddFunctionToCriteria(funcToAdd, FunctionAggregatorEnum.And);

            return this;
        }

        public ICanAddFunctionFilter<T> Or(Expression<Func<T, bool>> funcToAdd)
        {
            _baseQuery.AddFunctionToCriteria(funcToAdd, FunctionAggregatorEnum.Or);

            return this;
        }

        public ICanAddInclude<T> Include(Expression<Func<T, object>> spec)
        {
            _baseQuery.AddInclude(spec);

            return this;
        }

        public ICanAddInclude<T> Include(string spec)
        {
            _baseQuery.AddInclude(spec);

            return this;
        }

        public ICanAddThenOrderByOrTake<T> ThenOrderBy(string spec, bool sortDescending = false)
        {
            _baseQuery.AddOrder(new OrderBySpecification<string>(spec, sortDescending));

            return this;
        }

        public ICanAddThenOrderByOrTake<T> ThenOrderBy(Expression<Func<T, object>> spec, bool sortDescending = false)
        {
            _baseQuery.AddOrder(new OrderBySpecification<Expression<Func<T, object>>>(spec, sortDescending));

            return this;
        }

        public FluentQueryBuilder<T> WithSplitQuery(bool useSplitQuery = true)
        {
            _baseQuery.SplitQuery = useSplitQuery;

            return this;
        }

        public FluentQueryBuilder<T> UseSplitQuery(bool useSplitQuery = true)
        {
            return WithSplitQuery(useSplitQuery);
        }

        ICanAddPostBuilder<T> ICanAddPostBuilder<T>.WithSplitQuery(bool useSplitQuery) => WithSplitQuery(useSplitQuery);
        ICanAddPostBuilder<T> ICanAddPostBuilder<T>.UseSplitQuery(bool useSplitQuery) => WithSplitQuery(useSplitQuery);

        ICanAddInclude<T> ICanAddInclude<T>.WithSplitQuery(bool useSplitQuery) => WithSplitQuery(useSplitQuery);
        ICanAddInclude<T> ICanAddInclude<T>.UseSplitQuery(bool useSplitQuery) => WithSplitQuery(useSplitQuery);

        ICanBuild<T> ICanBuild<T>.WithSplitQuery(bool useSplitQuery) => WithSplitQuery(useSplitQuery);
        ICanBuild<T> ICanBuild<T>.UseSplitQuery(bool useSplitQuery) => WithSplitQuery(useSplitQuery);

        public FluentQueryBuilder<T> TagWith(string tag)
        {
            _baseQuery.TagWith = tag;

            return this;
        }

        public FluentQueryBuilder<T> WithTag(string tag)
        {
            return TagWith(tag);
        }

        ICanAddPostBuilder<T> ICanAddPostBuilder<T>.TagWith(string tag) => TagWith(tag);
        ICanAddPostBuilder<T> ICanAddPostBuilder<T>.WithTag(string tag) => TagWith(tag);

        ICanAddInclude<T> ICanAddInclude<T>.TagWith(string tag) => TagWith(tag);
        ICanAddInclude<T> ICanAddInclude<T>.WithTag(string tag) => TagWith(tag);

        ICanBuild<T> ICanBuild<T>.TagWith(string tag) => TagWith(tag);
        ICanBuild<T> ICanBuild<T>.WithTag(string tag) => TagWith(tag);
    }
}

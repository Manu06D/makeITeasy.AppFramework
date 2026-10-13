using makeITeasy.AppFramework.Models;
using Microsoft.EntityFrameworkCore;
using makeITeasy.AppFramework.Core.Extensions;

namespace makeITeasy.AppFramework.Infrastructure.EntityFramework.Persistence
{
    public static class SpecificationEvaluator<T> where T : class, IBaseEntity
    {
        public static IQueryable<T>? GetQuery(IQueryable<T> inputQuery, ISpecification<T> specification)
        {
            IQueryable<T>? query = inputQuery;

            // modify the IQueryable using the specification's criteria expression
            if (specification.Criteria != null)
            {
                query = query.Where(specification.Criteria);
            }

            if (specification.Includes != null && specification.Includes.Any())
            {
                query = specification.Includes.Aggregate(query,
                                        (current, include) => current.Include(include));
            }

            if (specification.IncludeStrings != null && specification.IncludeStrings.Any())
            {
                query = specification.IncludeStrings.Aggregate(query,
                                        (current, include) => current.Include(include));
            }

            if (specification.OrderBy?.Any() != null || specification.OrderByStrings?.Any() != null)
            {
                query = HandQueryForOrder(specification, query);
            }

#if NET10_0_OR_GREATER
            if (specification.SplitQuery.HasValue && query != null)
            {
                query = specification.SplitQuery.Value ? query.AsSplitQuery() : query.AsSingleQuery();
            }
#else
            if (specification.SplitQuery.GetValueOrDefault())
            {
                throw new NotSupportedException("SplitQuery is not supported in .NET versions prior to .NET 10.");
            }
#endif

            if (!string.IsNullOrWhiteSpace(specification.TagWith) && query != null)
            {
                query = query.TagWith(specification.TagWith);
            }

            return query;
        }

        private static IQueryable<T>? HandQueryForOrder(ISpecification<T> specification, IQueryable<T>? query)
        {
            ArgumentNullException.ThrowIfNull(query);

            if (specification?.OrderBy?.Count > 0)
            {
                IOrderedQueryable<T> sortedQuery =
                    specification.OrderBy[0].SortDescending ? query.OrderByDescending(specification.OrderBy.First().OrderBy) : query.OrderBy(specification.OrderBy.First().OrderBy);

                query = specification.OrderBy.Aggregate(sortedQuery,
                         (current, orderSpec) => orderSpec.SortDescending ? current.ThenByDescending(orderSpec.OrderBy) : current.ThenBy(orderSpec.OrderBy));
            }
            else if (specification?.OrderByStrings?.Count > 0)
            {
                IOrderedQueryable<T>? sortedQuery =
                    specification.OrderByStrings[0].SortDescending ? query.OrderByDescending(specification.OrderByStrings.First().OrderBy) : query.OrderBy(specification.OrderByStrings.First().OrderBy);

                query = specification.OrderByStrings.Aggregate(sortedQuery,
                    (current, orderSpec) => orderSpec.SortDescending ? current?.ThenByDescending(orderSpec.OrderBy) : current?.OrderBy(orderSpec.OrderBy));
            }

            return query;
        }
    }
}

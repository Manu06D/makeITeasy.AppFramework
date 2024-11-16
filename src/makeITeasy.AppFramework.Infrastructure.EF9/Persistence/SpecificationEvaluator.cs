using System.Linq;
using makeITeasy.AppFramework.Models;
using Microsoft.EntityFrameworkCore;
using makeITeasy.AppFramework.Core.Extensions;

namespace makeITeasy.AppFramework.Infrastructure.EF9.Persistence
{
    public static class SpecificationEvaluator<T> where T : class, IBaseEntity
    {
        public static IQueryable<T>? GetQuery(IQueryable<T> inputQuery, ISpecification<T> specification)
        {
            var query = inputQuery;

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

            return query;
        }

        private static IQueryable<T>? HandQueryForOrder(ISpecification<T> specification, IQueryable<T>? query)
        {
            if (query is null)
            {
                throw new ArgumentNullException(nameof(query));
            }

            if (specification?.OrderBy?.Count > 0)
            {
                IOrderedQueryable<T> sortedQuery = 
                    specification.OrderBy.First().SortDescending ? query.OrderByDescending(specification.OrderBy.First().OrderBy) : query.OrderBy(specification.OrderBy.First().OrderBy);

                query = specification.OrderBy.Aggregate(sortedQuery,
                         (current, orderSpec) => orderSpec.SortDescending ? current.ThenByDescending(orderSpec.OrderBy) : current.ThenBy(orderSpec.OrderBy));
            }
            else if (specification?.OrderByStrings?.Count > 0)
            {
                IOrderedQueryable<T>? sortedQuery = 
                    specification.OrderByStrings.First().SortDescending ? query.OrderByDescending(specification.OrderByStrings.First().OrderBy) : query.OrderBy(specification.OrderByStrings.First().OrderBy);

                query = specification.OrderByStrings.Aggregate(sortedQuery,
                    (current, orderSpec) => orderSpec.SortDescending ? current?.ThenByDescending(orderSpec.OrderBy) : current?.OrderBy(orderSpec.OrderBy));
            }


            return query;
        }
    }
}

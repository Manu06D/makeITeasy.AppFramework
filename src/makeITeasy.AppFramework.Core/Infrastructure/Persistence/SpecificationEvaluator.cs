using System;
using System.Linq;
using makeITeasy.AppFramework.Models;
using Microsoft.EntityFrameworkCore;
using makeITeasy.AppFramework.Core.Extensions;

namespace makeITeasy.AppFramework.Core.Infrastructure.Persistence
{
    public static class SpecificationEvaluator<T> where T : BaseEntity
    {
        public static IQueryable<T> GetQuery(IQueryable<T> inputQuery, ISpecification<T> specification)
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

        private static IQueryable<T> HandQueryForOrder(ISpecification<T> specification, IQueryable<T> query)
        {
            if (specification?.OrderBy != null)
            {
                var sortedQuery = specification.OrderBy.First().SortDescending ? query.OrderByDescending(specification.OrderBy.First().OrderBy) : query.OrderBy(specification.OrderBy.First().OrderBy);

                query = specification.OrderBy.Aggregate(sortedQuery,
                         (current, orderSpec) => orderSpec.SortDescending ? current.ThenByDescending(orderSpec.OrderBy) : current.ThenBy(orderSpec.OrderBy));
            }
            else  if (specification?.OrderByStrings != null)
            {
                query = specification.OrderByStrings.Aggregate(query, 
                    (current, orderSpec) => orderSpec.SortDescending ? current.OrderByDescending(orderSpec.OrderBy) : current.OrderBy(orderSpec.OrderBy));
            }

            return query;
        }
    }
}

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

            if (!String.IsNullOrEmpty(specification.OrderString))
            {
                if (specification.SortDescending)
                {
                    query = query.OrderByDescending(specification.OrderString);
                }
                else
                {
                    query = query.OrderBy(specification.OrderString);
                }
            }
            
            if(specification.Order != null)
            {
                if (specification.SortDescending)
                {
                    query = query.OrderByDescending(specification.Order);
                }
                else
                {
                    query = query.OrderBy(specification.Order);
                }
            }

            return query;
        }
    }
}

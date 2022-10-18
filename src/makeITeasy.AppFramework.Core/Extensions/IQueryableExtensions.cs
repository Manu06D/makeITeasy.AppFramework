using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace makeITeasy.AppFramework.Core.Extensions
{
    /// <summary>
    /// Source : https://stackoverflow.com/questions/1689199/c-sharp-code-to-order-by-a-property-using-the-property-name-as-a-string
    /// </summary>
    public static class IQueryableExtensions
    {
        public static IOrderedQueryable<T>? OrderBy<T>(this IQueryable<T> query, string propertyName, IComparer<object>? comparer = null)
        {
            return CallOrderedQueryable(query, "OrderBy", propertyName, comparer);
        }

        public static IOrderedQueryable<T>? OrderByDescending<T>(this IQueryable<T> query, string propertyName, IComparer<object>? comparer = null)
        {
            return CallOrderedQueryable(query, "OrderByDescending", propertyName, comparer);
        }

        public static IOrderedQueryable<T>? ThenBy<T>(this IOrderedQueryable<T> query, string propertyName, IComparer<object>? comparer = null)
        {
            return CallOrderedQueryable(query, "ThenBy", propertyName, comparer);
        }

        public static IOrderedQueryable<T>? ThenByDescending<T>(this IOrderedQueryable<T> query, string propertyName, IComparer<object>? comparer = null)
        {
            return CallOrderedQueryable(query, "ThenByDescending", propertyName, comparer);
        }

        /// <summary>
        /// Builds the Queryable functions using a TSource property name.
        /// </summary>
        private static IOrderedQueryable<T>? CallOrderedQueryable<T>(this IQueryable<T> query, string methodName, string propertyName, IComparer<object>? comparer = null)
        {
            if (query?.Provider is null)
            {
                throw new ArgumentNullException(nameof(query));
            }

            if (methodName is null)
            {
                throw new ArgumentNullException(nameof(methodName));
            }

            var param = Expression.Parameter(typeof(T), "x");

            var body = propertyName?.Split('.').Aggregate<string, Expression>(param, Expression.PropertyOrField);

            return CreateOrderedQuery(query, methodName, comparer, param, body);
        }

        private static IOrderedQueryable<T>? CreateOrderedQuery<T>(IQueryable<T> query, string methodName, IComparer<object>? comparer, ParameterExpression? param, Expression? body)
        {
            return (IOrderedQueryable<T>?)
                (comparer != null ?
                                query.Provider.CreateQuery(
                                Expression.Call(
                                    typeof(Queryable),
                                    methodName,
                                    new[] { typeof(T), body?.Type },
                                    query.Expression,
                                    Expression.Lambda(body, param),
                                    Expression.Constant(comparer)
                                )
                            )
                            :
                            query?.Provider.CreateQuery(
                                Expression.Call(
                                    typeof(Queryable),
                                    methodName,
                                    new[] { typeof(T), body?.Type },
                                    query.Expression,
                                    Expression.Lambda(body, param)
                                )
                            ));
        }
    }
}

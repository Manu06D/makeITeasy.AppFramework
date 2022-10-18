﻿using System;
using System.Linq.Expressions;

namespace makeITeasy.AppFramework.Core.Extensions
{
    public static class ExpressionExtensions
    {
        public static Expression<Func<T, bool>> Inverse<T>(this Expression<Func<T, bool>> expression)
        {
            if (expression is null)
            {
                throw new ArgumentNullException(nameof(expression));
            }

            return Expression.Lambda<Func<T, bool>>(Expression.Not(expression.Body), expression.Parameters[0]);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace makeITeasy.AppFramework.Core.Models
{
    public class UpdateDefinition<T>
    {
        private readonly List<(LambdaExpression Property, LambdaExpression ValueExpr)> _updates = new();

        public UpdateDefinition<T> Set<TProp>(Expression<Func<T, TProp>> property, TProp value)
        {
            Expression<Func<T, TProp>> valueExpr = _ => value!;
            _updates.Add((property, valueExpr));
            return this;
        }

        public UpdateDefinition<T> Set<TProp>(Expression<Func<T, TProp>> property, Expression<Func<T, TProp>> valueExpr)
        {
            _updates.Add((property, valueExpr));
            return this;
        }

        public IEnumerable<(LambdaExpression Property, LambdaExpression ValueExpr)> GetUpdates() => _updates;
    }
}

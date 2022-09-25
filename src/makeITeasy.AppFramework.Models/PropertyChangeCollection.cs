using System;
using System.Linq.Expressions;

namespace makeITeasy.AppFramework.Models
{
    public class PropertyChangeCollection<T, TProperty> where T:IBaseEntity where TProperty : class
    {
        public Expression<Func<T, TProperty>> Property { get; private set; }
        public Expression<Func<T, TProperty>> UpdateExpression { get; private set; }

        public PropertyChangeCollection(Expression<Func<T, TProperty>> property, Expression<Func<T, TProperty>> updateExpression)
        {
            Property = property;
            UpdateExpression = updateExpression;
        }
    }
}

using System;
using System.Linq.Expressions;

namespace makeITeasy.AppFramework.Models
{
    public class PropertyChangeCollection<T, TProperty> where T:IBaseEntity where TProperty : class
    {
        public Func<T, TProperty> Property { get; private set; }
        public Func<T, TProperty> UpdateExpression { get; private set; }

        public PropertyChangeCollection(Func<T, TProperty> property, Func<T, TProperty> updateExpression)
        {
            Property = property;
            UpdateExpression = updateExpression;
        }
    }
}

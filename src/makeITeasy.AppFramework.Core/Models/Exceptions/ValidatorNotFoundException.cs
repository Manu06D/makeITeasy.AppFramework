using System;
using System.Runtime.Serialization;

namespace makeITeasy.AppFramework.Core.Models.Exceptions
{
    public class ValidatorNotFoundException : Exception
    {
        private Type EntityType { get; set; }

        public ValidatorNotFoundException(Type entityType, string message = null, Exception innerException = null) 
            : base($"validator for {entityType.FullName} was not found", innerException)
        {
            EntityType = entityType;
        }

        protected ValidatorNotFoundException(Type entityType, SerializationInfo info, StreamingContext context) : base(info, context)
        {
            EntityType = entityType;
        }
    }
}

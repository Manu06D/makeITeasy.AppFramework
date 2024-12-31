using System;

namespace makeITeasy.AppFramework.Models.Exceptions
{
    public class InvalidSelectorException : Exception
    {
        public InvalidSelectorException() : base()
        {
        }

        public InvalidSelectorException(string message) : base(message)
        {
        }

        public InvalidSelectorException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}

using System;
using System.Runtime.Serialization;

namespace ImageClassification.API.Exceptions
{
    public class NotFoundClassifierException : Exception
    {
        public NotFoundClassifierException()
        {
        }

        public NotFoundClassifierException(string message) : base(message)
        {
        }

        public NotFoundClassifierException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected NotFoundClassifierException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}

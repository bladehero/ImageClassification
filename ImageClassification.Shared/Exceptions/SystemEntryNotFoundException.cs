using System;
using System.Runtime.Serialization;

namespace ImageClassification.Shared.Exceptions
{
    public class SystemEntryNotFoundException : Exception
    {
        public string Path { get; set; }

        public SystemEntryNotFoundException()
        {
        }

        public SystemEntryNotFoundException(string message) : base(message)
        {
        }

        public SystemEntryNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected SystemEntryNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}

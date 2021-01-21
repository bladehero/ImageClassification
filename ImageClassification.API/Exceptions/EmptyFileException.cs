using System;
using System.Runtime.Serialization;

namespace ImageClassification.API.Exceptions
{
    public class EmptyFileException : FormatException
    {
        public EmptyFileException() : base("File was empty")
        {
        }

        public EmptyFileException(string message) : base(message)
        {
        }

        public EmptyFileException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected EmptyFileException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}

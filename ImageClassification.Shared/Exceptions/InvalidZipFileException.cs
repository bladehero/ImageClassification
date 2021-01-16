using System;
using System.Runtime.Serialization;

namespace ImageClassification.Shared.Exceptions
{
    public class InvalidZipFileException : Exception
    {
        public string Path { get; set; }

        public InvalidZipFileException()
        {
        }

        public InvalidZipFileException(string message) : base(message)
        {
        }

        public InvalidZipFileException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected InvalidZipFileException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}

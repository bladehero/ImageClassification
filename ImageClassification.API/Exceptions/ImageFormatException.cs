using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace ImageClassification.API.Exceptions
{
    public class ImageFormatException : FormatException
    {
        public IEnumerable<string> CorrectFormats { get; set; }

        public ImageFormatException() : this("Image format was invalid")
        {
        }

        public ImageFormatException(params string[] formats)
            : this(formats.AsEnumerable())
        {
        }

        public ImageFormatException(IEnumerable<string> formats)
            : this($"Image formats can be used: {string.Join(",", formats)}")
        {
            CorrectFormats = formats;
        }

        public ImageFormatException(string message) : base(message)
        {
        }

        public ImageFormatException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected ImageFormatException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}

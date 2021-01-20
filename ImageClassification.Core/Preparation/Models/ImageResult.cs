using System.IO;

namespace ImageClassification.Core.Preparation.Models
{
    public struct ImageResult
    {
        public Stream Stream { get; set; }
        public string ContentType { get; set; }
    }
}

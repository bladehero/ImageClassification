using System.Collections.Generic;
using System.IO;

namespace ImageClassification.API.Extensions
{
    public static class StreamExtensions
    {
        public static async IAsyncEnumerable<string> ReadAllLinesAsync(this Stream stream)
        {
            using var reader = new StreamReader(stream);
            while (await reader.ReadLineAsync() is string line)
                yield return line;
        }
    }
}

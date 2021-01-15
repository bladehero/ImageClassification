using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace ImageClassification.Train.Common
{
    public class Web
    {
        public static async Task<bool> Download(string url, string directory, string file, IProgress<float> progress = null)
        {
            if (file == null)
                file = url.Split(Path.DirectorySeparatorChar).Last();

            Directory.CreateDirectory(directory);

            string relativeFilePath = Path.Combine(directory, file);

            if (File.Exists(relativeFilePath))
            {
                Console.WriteLine($"{relativeFilePath} already exists.");
                return false;
            }

            using (var client = new HttpClient())
            {
                using var stream = new FileStream(relativeFilePath, FileMode.Create, FileAccess.Write, FileShare.None);
                await client.DownloadAsync(url, stream, progress);
            }

            return true;
        }
    }
}

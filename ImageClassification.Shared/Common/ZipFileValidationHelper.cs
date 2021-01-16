using System.IO;
using System.IO.Compression;

namespace ImageClassification.Shared.Common
{
    public static class ZipFileValidationHelper
    {
        public static bool IsValid(string path)
        {
            try
            {
                using var zipFile = ZipFile.OpenRead(path);
                var entries = zipFile.Entries;
                return true;
            }
            catch (InvalidDataException)
            {
                return false;
            }
        }
    }
}

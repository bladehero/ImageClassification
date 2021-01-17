using ImageClassification.Shared.DataModels;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;

namespace ImageClassification.Shared
{
    public class FileUtils
    {
        public static IEnumerable<(Stream Stream, string Label)> LoadImagesFromArchive(string path, bool useFolderNameAsLabel)
        {
            var archive = ZipFile.OpenRead(path);

            var entries = archive.Entries
                                 .Where(x => Path.GetExtension(x.Name) == ".jpg"
                                          || Path.GetExtension(x.Name) == ".png");

            foreach (var entry in entries)
            {
                var stream = entry.Open();
                string label;
                if (useFolderNameAsLabel)
                {
                    label = Directory.GetParent(entry.FullName).Name;
                }
                else
                {
                    label = string.Concat(Path.GetFileName(entry.FullName)
                                              .TakeWhile(c => char.IsLetter(c)));
                }
                yield return (stream, label);
            }
        }

        public static IEnumerable<(string ImagePath, string Label)> LoadImagesFromDirectory(string folder, bool useFolderNameAsLabel)
        {
            var imagesPath = Directory
                .GetFiles(folder, "*", searchOption: SearchOption.AllDirectories)
                .Where(x => Path.GetExtension(x) == ".jpg" || Path.GetExtension(x) == ".jpeg" || Path.GetExtension(x) == ".png");

            foreach (var path in imagesPath)
            {
                string label;
                if (useFolderNameAsLabel)
                {
                    label = Directory.GetParent(path).Name;
                }
                else
                {
                    label = string.Concat(Path.GetFileName(path)
                                              .TakeWhile(c => char.IsLetter(c)));
                }
                yield return (path, label);
            }
        }

        public static IEnumerable<InMemoryImageData> LoadInMemoryImagesFromDirectory(
            string folder,
            bool useFolderNameAsLabel = true)
            => LoadImagesFromDirectory(folder, useFolderNameAsLabel)
                .Select(x => new InMemoryImageData(
                    image: File.ReadAllBytes(x.ImagePath),
                    label: x.Label,
                    imageFileName: Path.GetFileName(x.ImagePath)));

        public static string GetAbsolutePath(Assembly assembly, string relativePath)
        {
            var assemblyFolderPath = new FileInfo(assembly.Location).Directory.FullName;

            return Path.Combine(assemblyFolderPath, relativePath);
        }
    }
}

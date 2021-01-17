using ImageClassification.Core.Preparation;
using ImageClassification.Core.Preparation.Models;
using ImageClassification.Shared.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using Category = ImageClassification.Core.Preparation.Models.Category;

namespace ImageClassification.Preparation
{
    class Program
    {
        static async Task Main(string[] args)
        {
            #region Initialization
            var categories = new List<Category>
            {
                new Category{ Name = "portraits", Keywords = new List<string>{ "human", "face", "man", "woman" } },
                new Category{ Name = "scenery", Keywords = new List<string>{ "scenery", "landscape", "sky", "city", "street", "architecture" } },
                new Category{ Name = "food", Keywords = new List<string>{ "food", "fruits", "vegetables", "meal" } },
                new Category{ Name = "cars", Keywords = new List<string>{ "cars" } },
                new Category{ Name = "interior", Keywords = new List<string>{ "interior" } },
                new Category{ Name = "flowers", Keywords = new List<string>{ "flowers" } },
                new Category{ Name = "dogs", Keywords = new List<string>{ "dogs" } },
                new Category{ Name = "cats", Keywords = new List<string>{ "cats" } },
            };

            var parseRequest = new ParseRequest
            {
                Categories = categories,
                EstimatedCount = 100
            };

            const int maxWidth = 1920; 
            #endregion

            #region Paths

            #region Archive Settings
            const string defaultArchiveName = "data-set.zip";
            (char Left, char Right) indexBracers = ('(', ')');
            var pattern = defaultArchiveName.Split('.');
            #endregion

            #region Directories
            var currentDirectory = Directory.GetCurrentDirectory();
            var projectDirectory = Path.GetFullPath(Path.Combine(currentDirectory, "..", "..", ".."));
            var imagesDirectory = Path.Combine(projectDirectory, "assets", "images");
            #endregion

            Directory.CreateDirectory(imagesDirectory);

            #region Indexing
            var files = Directory.GetFiles(imagesDirectory);
            var currentIndex = files.Any()
                ? (int?)files.Max(path => GetIndexFromPath(path, pattern, indexBracers))
                : null;
            var nextIndex = currentIndex.HasValue ? (int?)currentIndex.Value + 1 : null;
            var indexString = nextIndex.HasValue ? $" {indexBracers.Left}{nextIndex}{indexBracers.Right}" : string.Empty;
            var archiveName = $"{pattern[0]}{indexString}.{pattern[^1]}";
            var archive = Path.Combine(imagesDirectory, archiveName);
            #endregion

            #endregion

            var context = new ParsingContext();
            var parsedImages = context.ParseImagesAsync(parseRequest);

            var stopwatch = Stopwatch.StartNew();

            using (var fs = new FileStream(archive, FileMode.CreateNew))
            {
                using var zip = new ZipArchive(fs, ZipArchiveMode.Update);
                var indexes = categories.SelectMany(x => x.Keywords).ToDictionary(x => x, x => default(int));

                await foreach (var parsedImage in parsedImages)
                {
                    if (!indexes.TryGetValue(parsedImage.Keyword, out int index))
                    {
                        throw new ArgumentException("There is no such category in search-list");
                    }

                    var image = parsedImage.Image;
                    var rawFormat = image.RawFormat;
                    if (maxWidth < image.Width)
                    {
                        image = image.ProportionalResizeImageWidth(maxWidth);
                    }

                    var format = new ImageFormatConverter().ConvertToString(rawFormat).ToLower();
                    var entryName = $"{parsedImage.Keyword}{(index == default ? string.Empty : $"-{index}")}.{format}";
                    var entryPath = Path.Combine(parsedImage.Category, entryName);
                    var entry = zip.CreateEntry(entryPath, CompressionLevel.Optimal);

                    using var stream = entry.Open();
                    image.Save(stream, rawFormat);

                    Console.WriteLine("Image {0}, Category: `{1}`, Keyword: `{2}` was parsed",
                                      index,
                                      parsedImage.Category,
                                      parsedImage.Keyword);

                    indexes[parsedImage.Keyword] += 1;
                }
            }

            stopwatch.Stop();

            Console.WriteLine();
            Console.WriteLine("Total process took:");
            Console.WriteLine(stopwatch.Elapsed);
            Console.WriteLine("Press anykey to exit...");
        }

        #region Helper
        private static decimal GetIndexFromPath(string path,
                                                string[] archivePattern,
                                                (char Left, char Right) indexBracers)
        {
            var file = Path.GetFileName(path);
            var extension = Path.GetExtension(file).TrimStart('.');
            if (file.StartsWith(archivePattern[0])
                && extension.Equals(archivePattern[^1], StringComparison.OrdinalIgnoreCase))
            {
                var name = file.Split('.')[0];
                if (name.Length > archivePattern[0].Length)
                {
                    var indexPart = name.Substring(archivePattern[0].Length).Trim();
                    if (indexPart.StartsWith(indexBracers.Left) && indexPart.EndsWith(indexBracers.Right))
                    {
                        var indexString = indexPart.TrimStart(indexBracers.Left).TrimEnd(indexBracers.Right);
                        if (int.TryParse(indexString, out int index))
                        {
                            return index;
                        }
                    }
                }
            }
            return decimal.Zero;
        }
        #endregion
    }
}

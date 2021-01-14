using ImageClassification.Preparation.Extensions;
using ImageClassification.Preparation.Models;
using ImageClassification.Preparation.Models.Unsplash;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace ImageClassification.Preparation
{
    class Program
    {
        static HttpClient httpClient = new HttpClient();

        const string url = @"https://unsplash.com/napi/search/photos";
        static class Folders
        {
            internal static string Root => Path.Combine("..", "..", "..");
            internal const string Assets = "assets";
            internal const string Images = "images";
            internal const string Categories = "categories";
            internal const string Tags = "tags";
        }

        static readonly List<Category> categories = new List<Category>
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

        const int pageSize = 30;
        const int chunkSize = 25;

        static readonly int startFrom = 1;

        /// <summary>
        /// If null - takes all of content.
        /// </summary>
        static readonly int imagesPerKeyword = 100;

        static async Task Main(string[] args)
        {
            var totalImageCount = imagesPerKeyword * categories.SelectMany(x => x.Keywords).Count();
            var currentImageCount = 0.0;
            var progress = new Progress<int>(e =>
            {
                currentImageCount += e;
                Console.WriteLine("{0}% - {1}/{2}", Math.Round(currentImageCount / totalImageCount * 100, 2), currentImageCount, totalImageCount);
            });

            var stopwatch = Stopwatch.StartNew();

            {
                var archive = PrepareArchive();

                var pageTasks = new List<(string Category, string Keyword, Task<Response> Task)>();
                foreach (var category in categories)
                {
                    foreach (var keyword in category.Keywords)
                    {
                        var uri = new Uri(url).AddParameter("query", keyword);
                        var total = (int)Math.Ceiling((double)imagesPerKeyword / pageSize);
                        var pages = Enumerable.Range(startFrom, total);

                        foreach (var page in pages)
                        {
                            var pageUri = uri.AddParameter("per_page", pageSize)
                                             .AddParameter("page", page);

                            var take = imagesPerKeyword - (page - 1) * pageSize;
                            var pageTask = Task.Run(async () =>
                            {
                                var response = await httpClient.GetAsync<Response>(pageUri);
                                response.Results = response.Results.Take(take);
                                return response;
                            });
                            pageTasks.Add((category.Name, keyword, pageTask));
                        }
                    }
                }

                await Task.WhenAll(pageTasks.Select(x => x.Task));

                var downloads = new List<(string Category, string Keyword, Result Result)>();
                foreach (var pageTask in pageTasks)
                {
                    foreach (var result in pageTask.Task.Result.Results)
                    {
                        if (downloads.All(x => x.Result.Id != result.Id))
                        {
                            downloads.Add((pageTask.Category, pageTask.Keyword, result));
                        }
                    }
                }

                foreach (var chunk in downloads.ChunkBy(chunkSize))
                {
                    await Task.WhenAll(chunk.Select(download
                        => Task.Run(async () => await DownloadToArchive(download.Category, download.Keyword, download.Result, archive, progress))));
                }

                archive.Dispose();
            }

            stopwatch.Stop();
            Console.WriteLine("Total time: {0}", stopwatch.Elapsed);
        }

        static ZipArchive PrepareArchive()
        {
            var directory = Path.Combine(Folders.Root, Folders.Assets, Folders.Images);
            Directory.CreateDirectory(directory);
            var path = Path.Combine(directory, Path.ChangeExtension(Folders.Categories, "zip"));
            var zipStream = new FileStream(path, FileMode.OpenOrCreate);
            return new ZipArchive(zipStream, ZipArchiveMode.Update);
        }

        static async Task DownloadToArchive(string category, string keyword, Result result, ZipArchive archive, IProgress<int> progress)
        {
            var file = Path.Combine(category,
                                    $"{Guid.NewGuid()}_" +
                                    $"{result.Id}_" +
                                    $"{result.Height}x{result.Width}.jpg");
            try
            {
                var downloadResponse = await httpClient.GetAsync(result.Links.Download);
                var entryStream = archive.CreateEntry(file, CompressionLevel.Optimal).Open();
                using var downloadStream = await downloadResponse.Content.ReadAsStreamAsync();
                await downloadStream.CopyToAsync(entryStream);
                progress.Report(1);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed: {0}", file);
            }
        }
    }
}

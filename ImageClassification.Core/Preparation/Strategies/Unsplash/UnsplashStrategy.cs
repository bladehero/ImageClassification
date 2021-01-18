﻿using ImageClassification.Core.Preparation.Models;
using ImageClassification.Core.Preparation.Strategies.Unsplash.Internal;
using ImageClassification.Shared.Common;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace ImageClassification.Core.Preparation.Strategies.Unsplash
{
    internal class UnsplashStrategy : IImageParsingStrategy
    {
        private const string url = @"https://unsplash.com/napi/search/photos";

        private readonly HttpClient httpClient = new HttpClient();

        private const int pageSize = 30;
        private const int startFrom = 1;

        public int MaxThreads { get; set; } = 4;

        public IEnumerable<ParsedImage> Parse(ParseRequest request, IProgress<float> progress = null)
        {
            var collection = ParseAsync(request, progress).ToListAsync().Result;
            return collection;
        }

        public async IAsyncEnumerable<ParsedImage> ParseAsync(ParseRequest request, IProgress<float> progress = null)
        {
            var imagesPerCategory = (double)request.EstimatedCount / request.Categories.Count();
            var capacity = (int)Math.Ceiling(imagesPerCategory / pageSize) * request.Categories.Sum(x => x.Keywords.Count());

            var throttler = new SemaphoreSlim(MaxThreads);
            foreach (var category in request.Categories)
            {
                var keywordsCount = category.Keywords.Count();
                var imagesPerKeyword = (int)Math.Ceiling(imagesPerCategory / keywordsCount);

                var allTasks = new List<Task>();
                var parsedImages = new List<ParsedImage>(imagesPerKeyword);

                var total = (int)Math.Ceiling((double)imagesPerKeyword / pageSize);
                var pages = Enumerable.Range(startFrom, total);

                var disposables = new List<IDisposable>();
                foreach (var keyword in category.Keywords)
                {
                    await throttler.WaitAsync();
                    allTasks.Add(
                        Task.Run(async () =>
                        {
                            try
                            {
                                var uri = new Uri(url).AddParameter("query", keyword);

                                foreach (var page in pages)
                                {
                                    var pageUri = uri.AddParameter("per_page", pageSize)
                                                     .AddParameter("page", page);

                                    var take = imagesPerKeyword - (page - 1) * pageSize;

                                    var response = await httpClient.GetAsync<Response>(pageUri);
                                    disposables.Add(response.Disposable);
                                    var results = response.Result.Results.Take(take);

                                    foreach (var result in results)
                                    {
                                        var download = await httpClient.GetAsync(result.Links.Download);
                                        var stream = await download.Content.ReadAsStreamAsync();
                                        var image = Image.FromStream(stream);
                                        var parsedImage = new ParsedImage
                                        {
                                            Category = category.Name,
                                            Image = image,
                                            Keyword = keyword
                                        };
                                        parsedImages.Add(parsedImage);
                                        disposables.Add(download);
                                        disposables.Add(stream);
                                    }
                                }
                            }
                            finally
                            {
                                throttler.Release();
                            }
                        }));
                }
                await Task.WhenAll(allTasks);

                foreach (var parsedImage in parsedImages)
                {
                    yield return parsedImage;
                }

                foreach (var disposable in disposables)
                {
                    disposable.Dispose();
                }
            }
        }
    }
}

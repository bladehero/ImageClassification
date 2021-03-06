﻿using ImageClassification.Core.Preparation.Interfaces;
using ImageClassification.Core.Preparation.Models;
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
        private const string _url = @"https://unsplash.com/napi/search/photos";

        private readonly HttpClient _httpClient = new HttpClient();

        private const int _pageSize = 30;
        private const int _startFrom = 1;

        public int MaxThreads { get; set; } = 16;

        async Task<ImageResult> IImageParsingStrategy.ParseContentAsync(string keyword, int index)
        {
            if (string.IsNullOrWhiteSpace(keyword))
            {
                ThrowHelper.Argument($"'{nameof(keyword)}' cannot be null or whitespace", nameof(keyword));
            }

            if (index < 0)
            {
                ThrowHelper.ArgumentOutOfRange(nameof(index), index, "Value must be 0 or greater!");
            }

            var uri = new Uri(_url).AddParameter("query", keyword)
                                  .AddParameter("per_page", 1)
                                  .AddParameter("page", index + 1);

            var response = await _httpClient.GetAsync<Response>(uri);
            var result = response.Result.Results.First();
            var download = await _httpClient.GetAsync(result.Links.Download);

            var stream = await download.Content.ReadAsStreamAsync();
            var contentType = download.Content.Headers.ContentType.MediaType;
            return new ImageResult { Stream = stream, ContentType = contentType };
        }

        IEnumerable<ParsedImage> IImageParsingStrategy.Parse(ParseRequest request, IProgress<ParseProgress> progress)
        {
            if (request is null)
            {
                ThrowHelper.Argument($"'{nameof(request)}' cannot be null or whitespace", nameof(request));
            }

            var collection = AsyncHelpers.RunSync(() => ((IImageParsingStrategy)this).ParseAsync(request, progress).ToListAsync());
            return collection;
        }

        async IAsyncEnumerable<ParsedImage> IImageParsingStrategy.ParseAsync(ParseRequest request, IProgress<ParseProgress> progress)
        {
            if (request is null)
            {
                ThrowHelper.ArgumentNull(nameof(request));
            }

            if (request.EstimatedCount < 1)
            {
                ThrowHelper.ArgumentOutOfRange(nameof(request.EstimatedCount), request.EstimatedCount, "Value must be 1 or greater!");
            }


            var imagesPerCategory = (double)request.EstimatedCount / request.Categories.Count();
            var capacity = (int)Math.Ceiling(imagesPerCategory / _pageSize) * request.Categories.Sum(x => x.Keywords.Count());

            var throttler = new SemaphoreSlim(MaxThreads);
            var currentCount = 0;
            foreach (var category in request.Categories)
            {
                var keywordsCount = category.Keywords.Count();
                var imagesPerKeyword = (int)Math.Ceiling(imagesPerCategory / keywordsCount);

                var allTasks = new List<Task>();
                var parsedImages = new List<ParsedImage>(imagesPerKeyword);

                var total = (int)Math.Ceiling((double)imagesPerKeyword / _pageSize);
                var pages = Enumerable.Range(_startFrom, total);

                var disposables = new List<IDisposable>();
                foreach (var keyword in category.Keywords)
                {
                    await throttler.WaitAsync();
                    allTasks.Add(
                        Task.Run(async () =>
                        {
                            try
                            {
                                var uri = new Uri(_url).AddParameter("query", keyword);

                                foreach (var page in pages)
                                {
                                    var pageUri = uri.AddParameter("per_page", _pageSize)
                                                     .AddParameter("page", page);

                                    var take = imagesPerKeyword - (page - 1) * _pageSize;

                                    var response = await _httpClient.GetAsync<Response>(pageUri);
                                    disposables.Add(response.Disposable);
                                    var results = response.Result.Results.Take(take);

                                    foreach (var result in results)
                                    {
                                        var download = await _httpClient.GetAsync(result.Links.Download);
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
                    var data = new ParseProgress
                    {
                        CurrentCount = ++currentCount,
                        EstimatedCount = request.EstimatedCount
                    };
                    progress?.Report(data);
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

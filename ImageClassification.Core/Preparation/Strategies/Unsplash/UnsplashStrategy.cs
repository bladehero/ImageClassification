using ImageClassification.Core.Preparation.Models;
using ImageClassification.Core.Preparation.Strategies.Unsplash.Internal;
using ImageClassification.Shared.Common;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace ImageClassification.Core.Preparation.Strategies.Unsplash
{
    internal class UnsplashStrategy : IImageParsingStrategy
    {
        private const string url = @"https://unsplash.com/napi/search/photos";

        private readonly HttpClient httpClient = new HttpClient();

        private const int pageSize = 30;
        private const int startFrom = 1;

        public IEnumerable<ParsedImage> Parse(ParseRequest request, IProgress<float> progress = null)
        {
            var collection = ParseAsync(request, progress).ToListAsync().Result;
            return collection;
        }

        public async IAsyncEnumerable<ParsedImage> ParseAsync(ParseRequest request, IProgress<float> progress = null)
        {
            var imagesPerCategory = (double)request.EstimatedCount / request.Categories.Count();
            var capacity = (int)Math.Ceiling(imagesPerCategory / pageSize) * request.Categories.Sum(x => x.Keywords.Count());
            var tasks = new List<Task<IAsyncEnumerable<ParsedImage>>>(capacity);
            foreach (var category in request.Categories)
            {
                var imagesPerKeyword = (int)Math.Ceiling(imagesPerCategory / category.Keywords.Count());

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

                        var task = httpClient.GetAsync<Response>(pageUri)
                                             .ContinueWith(response =>
                                             {
                                                 var results = response.Result.Results.Take(take);
                                                 return _parseHelper(results, category.Name, keyword);
                                             });
                        tasks.Add(task);
                    }
                }
            }

            var completed = await Task.WhenAll(tasks);

            foreach (var enumerable in completed)
            {
                await foreach (var parsedImage in enumerable)
                {
                    yield return parsedImage;
                }
            }
        }

        private async IAsyncEnumerable<ParsedImage> _parseHelper(IEnumerable<Result> results,
                                                                 string category,
                                                                 string keyword)
        {
            foreach (var result in results)
            {
                var parsedImageTask =
                httpClient.GetAsync(result.Links.Download)
                          .ContinueWith(download =>
                          {
                              return
                              download.Result
                                      .Content
                                      .ReadAsStreamAsync()
                                      .ContinueWith(streamResponse =>
                                      {
                                          var image = Image.FromStream(streamResponse.Result);
                                          return new ParsedImage
                                          {
                                              Category = category,
                                              Image = image,
                                              Keyword = keyword
                                          };
                                      });
                          })
                          .Unwrap();
                yield return await parsedImageTask;
            }
        }
    }
}

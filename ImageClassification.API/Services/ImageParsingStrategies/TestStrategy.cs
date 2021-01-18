using ImageClassification.API.Configurations;
using ImageClassification.Core.Preparation.Interfaces;
using ImageClassification.Core.Preparation.Models;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;

namespace ImageClassification.API.Services.ImageParsingStrategies
{
    public class TestStrategy : IImageParsingStrategy
    {
        private readonly MLModelOptions _options;

        public const string Category = "Test";
        public const string Keyword = "Test";

        public TestStrategy(IOptions<MLModelOptions> options)
        {
            _options = options.Value;
        }

        Task<(Stream Stream, string ContentType)> IImageParsingStrategy.ParseContentAsync(string keyword, int index)
        {
            Stream stream = File.OpenRead(_options.WarmupImagePath);
            new FileExtensionContentTypeProvider().TryGetContentType(_options.WarmupImagePath, out string contentType);
            return Task.FromResult((stream, contentType));
        }

        IEnumerable<ParsedImage> IImageParsingStrategy.Parse(ParseRequest request, IProgress<ParseProgress> progress)
        {
            return new ParsedImage[] 
            {
                new ParsedImage 
                {
                    Category = Category,
                    Image = Image.FromFile(_options.WarmupImagePath),
                    Keyword = Keyword
                }
            };
        }

        async IAsyncEnumerable<ParsedImage> IImageParsingStrategy.ParseAsync(ParseRequest request, IProgress<ParseProgress> progress)
        {
            var collection = await Task.FromResult(new ParsedImage[]
            {
                new ParsedImage
                {
                    Category = Category,
                    Image = Image.FromFile(_options.WarmupImagePath),
                    Keyword = Keyword
                }
            });
            foreach (var image in collection)
            {
                yield return image;
            }
        }
    }
}

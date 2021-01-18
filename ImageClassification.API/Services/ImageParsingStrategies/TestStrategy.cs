using ImageClassification.API.Configurations;
using ImageClassification.Core.Preparation.Interfaces;
using ImageClassification.Core.Preparation.Models;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Drawing;
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

        Task<Image> IImageParsingStrategy.Parse(string keyword, int index)
        {
            return Task.FromResult(Image.FromFile(_options.WarmupImagePath));
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

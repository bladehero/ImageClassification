using ImageClassification.Core.Preparation.Interfaces;
using ImageClassification.Core.Preparation.Models;
using ImageClassification.Core.Preparation.Strategies.Unsplash;
using ImageClassification.Shared.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace ImageClassification.Core.Preparation
{
    public class ParsingContext : IParsingContext
    {
        private IImageParsingStrategy imageParsingStrategy;

        public IImageParsingStrategy Default => new UnsplashStrategy();

        public IImageParsingStrategy ImageParsingStrategy
        {
            set
            {
                if (value is null)
                {
                    ThrowHelper.NullReference(nameof(ImageParsingStrategy));
                }

                imageParsingStrategy = value;
            }
        }

        public ParsingContext()
        {
            ImageParsingStrategy = Default;
        }

        public ParsingContext(IImageParsingStrategy imageParsingStrategy) : this()
        {
            ImageParsingStrategy = imageParsingStrategy;
        }
        public async Task<(Stream Stream, string ContentType)> ParseImageAsync(string keyword, int index)
        {
            var result = await imageParsingStrategy.ParseContentAsync(keyword, index);
            return result;
        }

        public IEnumerable<ParsedImage> ParseImages(ParseRequest request, IProgress<ParseProgress> progress = null)
        {
            var result = imageParsingStrategy.Parse(request, progress);
            return result;
        }

        public IAsyncEnumerable<ParsedImage> ParseImagesAsync(ParseRequest request, IProgress<ParseProgress> progress = null)
        {
            var result = imageParsingStrategy.ParseAsync(request, progress);
            return result;
        }
    }
}

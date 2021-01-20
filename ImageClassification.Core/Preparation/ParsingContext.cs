using ImageClassification.Core.Preparation.Interfaces;
using ImageClassification.Core.Preparation.Models;
using ImageClassification.Core.Preparation.Strategies.Unsplash;
using ImageClassification.Shared.Common;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ImageClassification.Core.Preparation
{
    public class ParsingContext : IParsingContext
    {
        private IImageParsingStrategy _imageParsingStrategy;

        public IImageParsingStrategy Default => new UnsplashStrategy();

        public IImageParsingStrategy ImageParsingStrategy
        {
            set
            {
                if (value is null)
                {
                    ThrowHelper.NullReference(nameof(ImageParsingStrategy));
                }

                _imageParsingStrategy = value;
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
        public async Task<ImageResult> ParseImageAsync(string keyword, int index)
        {
            var result = await _imageParsingStrategy.ParseContentAsync(keyword, index);
            return result;
        }

        public IEnumerable<ParsedImage> ParseImages(ParseRequest request, IProgress<ParseProgress> progress = null)
        {
            var result = _imageParsingStrategy.Parse(request, progress);
            return result;
        }

        public IAsyncEnumerable<ParsedImage> ParseImagesAsync(ParseRequest request, IProgress<ParseProgress> progress = null)
        {
            var result = _imageParsingStrategy.ParseAsync(request, progress);
            return result;
        }
    }
}

using ImageClassification.Core.Preparation.Models;
using ImageClassification.Core.Preparation.Strategies.Unsplash;
using ImageClassification.Shared.Common;
using System;
using System.Collections.Generic;

namespace ImageClassification.Core.Preparation
{
    public class ParsingContext
    {
        private IImageParsingStrategy imageParsingStrategy;

        /// <summary>
        /// 
        /// </summary>
        public Progress<float> Progress { get; }

        /// <summary>
        /// 
        /// </summary>
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

        /// <summary>
        /// 
        /// </summary>
        public ParsingContext()
        {
            Progress = new Progress<float>();
            ImageParsingStrategy = new UnsplashStrategy();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="imageParsingStrategy"></param>
        public ParsingContext(IImageParsingStrategy imageParsingStrategy) : this()
        {
            ImageParsingStrategy = imageParsingStrategy;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public IEnumerable<ParsedImage> ParseImages(ParseRequest request)
        {
            var result = imageParsingStrategy.Parse(request, Progress);
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public IAsyncEnumerable<ParsedImage> ParseImagesAsync(ParseRequest request)
        {
            var result = imageParsingStrategy.ParseAsync(request, Progress);
            return result;
        }
    }
}

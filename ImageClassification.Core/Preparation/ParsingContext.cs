using ImageClassification.Core.Preparation.Models;
using ImageClassification.Core.Preparation.Strategies.Unsplash;
using System;
using System.Collections.Generic;

namespace ImageClassification.Core.Preparation
{
    public class ParsingContext
    {
        /// <summary>
        /// 
        /// </summary>
        public Progress<float> Progress { get; }

        /// <summary>
        /// 
        /// </summary>
        public IImageParsingStrategy ImageParsingStrategy { private get; set; }

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
        public IAsyncEnumerable<ParsedImage> ParseImages(ParseRequest request)
        {
            var result = ImageParsingStrategy.ParseAsync(request, Progress);
            return result;
        }
    }
}

using ImageClassification.Core.Preparation.Models;
using ImageClassification.Core.Preparation.Strategies.Unsplash;
using ImageClassification.Shared.Common;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;

namespace ImageClassification.Core.Preparation
{
    public class ParsingContext
    {
        private IImageParsingStrategy imageParsingStrategy;

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
        /// <param name="keyword"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public async Task<Image> ParseImageAsync(string keyword, int index)
        {
            var result = await imageParsingStrategy.Parse(keyword, index);
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="progress"></param>
        /// <returns></returns>
        public IEnumerable<ParsedImage> ParseImages(ParseRequest request, IProgress<ParseProgress> progress = null)
        {
            var result = imageParsingStrategy.Parse(request, progress);
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="progress"></param>
        /// <returns></returns>
        public IAsyncEnumerable<ParsedImage> ParseImagesAsync(ParseRequest request, IProgress<ParseProgress> progress = null)
        {
            var result = imageParsingStrategy.ParseAsync(request, progress);
            return result;
        }
    }
}

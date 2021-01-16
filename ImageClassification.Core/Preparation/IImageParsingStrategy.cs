using ImageClassification.Core.Preparation.Models;
using System;
using System.Collections.Generic;

namespace ImageClassification.Core.Preparation
{
    public interface IImageParsingStrategy
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="progress"></param>
        /// <returns></returns>
        IEnumerable<ParsedImage> Parse(ParseRequest request, IProgress<float> progress = null);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="progress"></param>
        /// <returns></returns>
        IAsyncEnumerable<ParsedImage> ParseAsync(ParseRequest request, IProgress<float> progress = null);
    }
}

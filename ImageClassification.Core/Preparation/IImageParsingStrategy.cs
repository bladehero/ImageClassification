using ImageClassification.Core.Preparation.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;

namespace ImageClassification.Core.Preparation
{
    public interface IImageParsingStrategy
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="keyword"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        Task<Image> Parse(string keyword, int index);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="progress"></param>
        /// <returns></returns>
        IEnumerable<ParsedImage> Parse(ParseRequest request, IProgress<ParseProgress> progress = null);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="progress"></param>
        /// <returns></returns>
        IAsyncEnumerable<ParsedImage> ParseAsync(ParseRequest request, IProgress<ParseProgress> progress = null);
    }
}

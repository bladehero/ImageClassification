using ImageClassification.Core.Preparation.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;

namespace ImageClassification.Core.Preparation.Interfaces
{
    public interface IParsingContext
    {
        /// <summary>
        /// Default strategy.
        /// </summary>
        /// <remarks>
        /// Uses <see href="https://unsplash.com/developers">Unsplash API</see> as external resource.
        /// </remarks>
        IImageParsingStrategy Default { get; }

        /// <summary>
        /// Selected strategy for image parsing.
        /// </summary>
        /// <remarks>
        /// Can be switched on demand.
        /// </remarks>
        IImageParsingStrategy ImageParsingStrategy { set; }

        /// <summary>
        /// Returns a task for getting image.
        /// </summary>
        /// <param name="keyword">Keyword to search.</param>
        /// <param name="index">Index of sequence images.</param>
        /// <returns>Stream converted to an image.</returns>
        Task<Image> ParseImageAsync(string keyword, int index);

        /// <summary>
        /// Creating a request for external API for getting collection of images by a specific request data.
        /// </summary>
        /// <param name="request">Request for parsing data.</param>
        /// <param name="progress">Progress reporter.</param>
        /// <returns>Returns a collection of images.</returns>
        IEnumerable<ParsedImage> ParseImages(ParseRequest request, IProgress<ParseProgress> progress = null);

        /// <summary>
        /// Creating a request for external API for getting collection of images by a specific request data.
        /// </summary>
        /// <param name="request">Request for parsing data.</param>
        /// <param name="progress">Progress reporter.</param>
        /// <returns>Returns an asynchronous collection of images.</returns>
        IAsyncEnumerable<ParsedImage> ParseImagesAsync(ParseRequest request, IProgress<ParseProgress> progress = null);
    }
}
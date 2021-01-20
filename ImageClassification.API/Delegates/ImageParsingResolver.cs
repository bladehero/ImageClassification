using ImageClassification.API.Enums;
using ImageClassification.Core.Preparation.Interfaces;

namespace ImageClassification.API.Delegates
{
    /// <summary>
    /// Delegate for resolving selection of possible image parsing strategies per request.
    /// </summary>
    /// <param name="key">Strategy key.</param>
    /// <returns>Image parsing strategy.</returns>
    public delegate IImageParsingStrategy ImageParsingResolver(ImageParsingStrategy key = ImageParsingStrategy.DefaultImageParsing);
}

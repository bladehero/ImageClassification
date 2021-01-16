using ImageClassification.Core.Preparation.Models;
using System;
using System.Collections.Generic;

namespace ImageClassification.Core.Preparation
{
    public interface IImageParsingStrategy
    {
        IEnumerable<ParsedImage> Parse(ParseRequest request, IProgress<float> progress = null);

        IAsyncEnumerable<ParsedImage> ParseAsync(ParseRequest request, IProgress<float> progress = null);
    }
}

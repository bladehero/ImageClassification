using ImageClassification.API.Configurations;
using ImageClassification.API.Delegates;
using ImageClassification.API.Enums;
using ImageClassification.API.Interfaces;
using ImageClassification.Core.Preparation.Interfaces;
using ImageClassification.Core.Preparation.Models;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace ImageClassification.API.Services
{
    public class ImageSourceService : IImageSourceService
    {
        private readonly ImageParsingResolver _imageParsingResolver;
        private readonly IParsingContext _parsingContext;

        public ImageSourceService(ImageParsingResolver imageParsingResolver,
                                  IParsingContext parsingContext)
        {
            _imageParsingResolver = imageParsingResolver;
            _parsingContext = parsingContext;
        }

        public void ChangeParsingStrategy(ImageParsingStrategy key)
        {
            if (_imageParsingResolver?.Invoke(key) is IImageParsingStrategy strategy)
            {
                _parsingContext.ImageParsingStrategy = strategy;
            }
        }

        public async Task<ImageResult> ParseSingleImageAsync(string keyword, int index)
        {
            var result = await _parsingContext.ParseImageAsync(keyword, index);
            return result;
        }
    }
}

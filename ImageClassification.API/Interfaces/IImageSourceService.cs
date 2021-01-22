using ImageClassification.API.Enums;
using ImageClassification.Core.Preparation.Models;
using System.Threading.Tasks;

namespace ImageClassification.API.Interfaces
{
    public interface IImageSourceService
    {
        void ChangeParsingStrategy(ImageParsingStrategy key);
        Task<ImageResult> ParseSingleImageAsync(string keyword, int index);
    }
}
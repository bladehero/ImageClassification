using ImageClassification.API.Models;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ImageClassification.API.Interfaces
{
    public interface IClassificationService
    {
        Task<ClassificationPredictionVM> Classify(string classifier, IFormFile imageFile);
        IAsyncEnumerable<string> GetPossibleClassifications(string classifier);
    }
}
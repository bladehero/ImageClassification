using ImageClassification.Core.Train;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace ImageClassification.API.Interfaces
{
    public interface IClassifierService
    {
        IEnumerable<string> GetAllClassifiers();
        Stream GetClassifierStream(string classifier);
        Task<ITrainWrapper> TrainClassifier(string imageFolder, string classifier);
    }
}
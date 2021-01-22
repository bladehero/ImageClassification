using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace ImageClassification.API.Interfaces
{
    public interface IClassifierService
    {
        IEnumerable<string> GetAllClassifiers();
        Stream GetClassifierStream(string classifier);
        void DeleteClassifier(string classifier);
        Task TrainClassifier(string imageFolder, string classifier);
    }
}
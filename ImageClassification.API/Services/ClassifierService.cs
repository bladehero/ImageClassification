using ImageClassification.API.Configurations;
using ImageClassification.API.Global;
using ImageClassification.API.Interfaces;
using ImageClassification.Core.Train;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ImageClassification.API.Services
{
    public class ClassifierService : IClassifierService
    {
        private readonly MLModelOptions _mlOptions;
        private readonly StorageOptions _storageOptions;
        private readonly IHostEnvironment _hostingEnvironment;
        private readonly ITrainProxyWrapper _trainProxyWrapper;

        public ClassifierService(IOptions<MLModelOptions> mlOptions,
                                 IOptions<StorageOptions> storageOptions,
                                 IHostEnvironment hostingEnvironment,
                                 ITrainProxyWrapper trainProxyWrapper)
        {
            _mlOptions = mlOptions.Value;
            _storageOptions = storageOptions.Value;
            _hostingEnvironment = hostingEnvironment;
            _trainProxyWrapper = trainProxyWrapper;
        }

        public IEnumerable<string> GetAllClassifiers()
        {
            var path = _mlOptions.MLModelFilePath;
            if (!Directory.Exists(path))
            {
                throw new DirectoryNotFoundException("Not found repository for storaging classifiers");
            }

            var classifiers = Directory.GetFiles(path,
                                                 $"*{Constants.Extensions.Zip}",
                                                 SearchOption.AllDirectories);
            if (classifiers.Length == 0)
            {
                throw new FileNotFoundException("System has no classifiers yet!");
            }

            return classifiers.Select(path => Path.GetFileNameWithoutExtension(path));
        }

        public Stream GetClassifierStream(string classifier)
        {
            var path = Path.Join(_mlOptions.MLModelFilePath, $"{classifier}{Constants.Extensions.Zip}");
            if (!File.Exists(path))
            {
                throw new FileNotFoundException("System has no classifiers yet with the specified name!", classifier);
            }
            return File.OpenRead(path);
        }

        public async Task<ITrainWrapper> TrainClassifier(string imageFolder, string classifier)
        {
            if (string.IsNullOrWhiteSpace(imageFolder))
            {
                throw new ArgumentException($"'{nameof(imageFolder)}' cannot be null or whitespace", nameof(imageFolder));
            }

            if (string.IsNullOrWhiteSpace(classifier))
            {
                throw new ArgumentException($"'{nameof(classifier)}' cannot be null or whitespace", nameof(classifier));
            }

            var imageFolderPath = Path.Combine(_hostingEnvironment.ContentRootPath, _storageOptions.StoragePath, imageFolder);
            _trainProxyWrapper.Path = imageFolderPath;

            var pathToSave = Path.Combine(_mlOptions.MLModelFilePath, Path.ChangeExtension(classifier, Constants.Extensions.Zip));
            await _trainProxyWrapper.TrainAsync(pathToSave);

            return _trainProxyWrapper;
        }
    }
}

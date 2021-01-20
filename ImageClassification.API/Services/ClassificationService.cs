using ImageClassification.API.Configurations;
using ImageClassification.API.Exceptions;
using ImageClassification.API.Extensions;
using ImageClassification.API.Global;
using ImageClassification.API.Interfaces;
using ImageClassification.API.Models;
using ImageClassification.Core.Train;
using ImageClassification.Shared.DataModels;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.ML;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;

namespace ImageClassification.API.Services
{
    public class ClassificationService : IClassificationService
    {
        private readonly ILogger<ClassificationService> _logger;
        private readonly IPredictionEnginePoolService<InMemoryImageData, ImagePrediction> _predictionEnginePoolService;
        private readonly MLModelOptions _mlOptions;

        public ClassificationService(ILogger<ClassificationService> logger,
                                     IPredictionEnginePoolService<InMemoryImageData, ImagePrediction> predictionEnginePoolService,
                                     IOptions<MLModelOptions> mlOptions)
        {
            _mlOptions = mlOptions.Value;
            _logger = logger;
            _predictionEnginePoolService = predictionEnginePoolService;
        }

        public IEnumerable<string> GetAllClassifiers()
        {
            var path = _mlOptions.MLModelFilePath;
            if (!Directory.Exists(path))
            {
                throw new DirectoryNotFoundException("Not found repository for storaging classifiers");
            }

            var classifiers = Directory.GetFiles(path, "*.zip", SearchOption.AllDirectories);
            if (classifiers.Length == 0)
            {
                throw new FileNotFoundException("System has no classifiers yet!");
            }

            return classifiers.Select(path => Path.GetFileNameWithoutExtension(path));
        }

        public IAsyncEnumerable<string> GetPossibleClassifications(string classifier)
        {
            if (string.IsNullOrWhiteSpace(classifier))
            {
                throw new ArgumentException($"'{nameof(classifier)}' cannot be null or whitespace", nameof(classifier));
            }

            var path = _mlOptions.MLModelFilePath;
            var fileName = Path.Combine(path, Path.ChangeExtension(classifier, Constants.Extensions.Zip));
            if (!File.Exists(fileName))
            {
                throw new FileNotFoundException($"Classifier with the name `{classifier}` not found!", fileName);
            }

            var zip = ZipFile.OpenRead(fileName);
            var classifications = zip.GetEntry(DefaultTrainWrapper.ClassificationsFileName)
                                     .Open()
                                     .ReadAllLinesAsync();
            return classifications;
        }

        public async Task<ClassificationPredictionVM> Classify(string classifier, IFormFile imageFile)
        {
            if (imageFile.Length == 0)
            {
                throw new EmptyFileException();
            }

            var imageMemoryStream = new MemoryStream();
            await imageFile.CopyToAsync(imageMemoryStream);

            var imageData = imageMemoryStream.ToArray();
            if (!imageData.IsValidImage())
            {
                throw new ImageFormatException(ImageExtensions.CanBeUsed.Select(x => x.GetDescription()));
            }

            _logger.LogInformation("Start processing image...");

            var watch = System.Diagnostics.Stopwatch.StartNew();

            var imageInputData = new InMemoryImageData(imageData);

            _predictionEnginePoolService.Get(classifier, out PredictionEngine<InMemoryImageData, ImagePrediction> engine);
            if (engine is null)
            {
                throw new NotFoundClassifierException($"Classifier `{classifier}` is not found");
            }

            var prediction = engine.Predict(imageInputData);

            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            _logger.LogInformation($"Image processed in {elapsedMs} miliseconds");

            var imageBestLabelPrediction = new ClassificationPredictionVM
            {
                PredictedLabel = prediction.PredictedLabel,
                Probability = prediction.Score.Max(),
                PredictionExecutionTime = elapsedMs,
                ImageId = imageFile.FileName,
            };
            return imageBestLabelPrediction;
        }
    }
}

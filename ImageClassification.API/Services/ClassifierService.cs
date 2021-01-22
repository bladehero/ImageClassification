using ImageClassification.API.Configurations;
using ImageClassification.API.Global;
using ImageClassification.API.Hubs;
using ImageClassification.API.Interfaces;
using Microsoft.AspNetCore.SignalR;
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
        private readonly IHubContext<TrainLogHub> _trainLogHub;

        public ClassifierService(IOptions<MLModelOptions> mlOptions,
                                 IOptions<StorageOptions> storageOptions,
                                 IHostEnvironment hostingEnvironment,
                                 ITrainProxyWrapper trainProxyWrapper,
                                 IHubContext<TrainLogHub> trainLogHub)
        {
            _mlOptions = mlOptions.Value;
            _storageOptions = storageOptions.Value;
            _hostingEnvironment = hostingEnvironment;
            _trainProxyWrapper = trainProxyWrapper;
            _trainLogHub = trainLogHub;
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

        public void DeleteClassifier(string classifier)
        {
            if (string.IsNullOrWhiteSpace(classifier))
            {
                throw new ArgumentException($"'{nameof(classifier)}' cannot be null or whitespace", nameof(classifier));
            }

            var path = Path.Join(_mlOptions.MLModelFilePath, $"{classifier}{Constants.Extensions.Zip}");
            if (!File.Exists(path))
            {
                throw new FileNotFoundException("While deleting file wasn't found!", classifier);
            }

            File.Delete(path);
        }

        public async Task TrainClassifier(string imageFolder, string classifier)
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

            _trainProxyWrapper.ProgressChanged += TrainProxyWrapper_ProgressChanged;
            _trainProxyWrapper.MulticlassMetricsUpdated += TrainProxyWrapper_MulticlassMetricsUpdated;
            _trainProxyWrapper.ImageMetricsUpdated += TrainProxyWrapper_ImageMetricsUpdated;

            _trainProxyWrapper.MeasureTime = true;

            var pathToSave = Path.Combine(_mlOptions.MLModelFilePath, Path.ChangeExtension(classifier, Constants.Extensions.Zip));
            await _trainProxyWrapper.TrainAsync(pathToSave);
        }

        #region Event Handlers
        private async void TrainProxyWrapper_ImageMetricsUpdated(Microsoft.ML.Vision.ImageClassificationTrainer.ImageClassificationMetrics metrics)
        {
            if (metrics.Train is Microsoft.ML.Vision.ImageClassificationTrainer.TrainMetrics trainMetrics)
            {
                await _trainLogHub.Clients.All.SendAsync("Log", string.Format("Accuracy: {0}", trainMetrics.Accuracy));
                await _trainLogHub.Clients.All.SendAsync("Log", string.Format("- Accuracy of the batch on this Epoch. Higher the better."));
                await _trainLogHub.Clients.All.SendAsync("Log", string.Format("Batch Processed Count: {0}", trainMetrics.BatchProcessedCount));
                await _trainLogHub.Clients.All.SendAsync("Log", string.Format("- The number of batches processed in an epoch."));
                await _trainLogHub.Clients.All.SendAsync("Log", string.Format("Cross-Entropy: {0}", trainMetrics.CrossEntropy));
                await _trainLogHub.Clients.All.SendAsync("Log", string.Format("- Cross-Entropy (loss) of the batch on this Epoch. Lower the better."));
                await _trainLogHub.Clients.All.SendAsync("Log", string.Format("Epoch: {0}", trainMetrics.Epoch));
                await _trainLogHub.Clients.All.SendAsync("Log", string.Format("- The training epoch index for which this metric is reported."));
                await _trainLogHub.Clients.All.SendAsync("Log", string.Format("Learning Rate: {0}", trainMetrics.LearningRate));
                await _trainLogHub.Clients.All.SendAsync("Log", string.Format("- Learning Rate used for this Epoch. Changes for learning rate scheduling."));
            }

            if (metrics.Bottleneck is Microsoft.ML.Vision.ImageClassificationTrainer.BottleneckMetrics bottleneckMetrics)
            {
                await _trainLogHub.Clients.All.SendAsync("Log", string.Format("Type of metrics: {0}", bottleneckMetrics.DatasetUsed));
                await _trainLogHub.Clients.All.SendAsync("Log", string.Format("Image with index {0} was processed out.", bottleneckMetrics.Index));
            }
        }
        private async void TrainProxyWrapper_ProgressChanged(Core.Train.Models.TrainProgress progress)
        {
            await _trainLogHub.Clients.All.SendAsync("Log", $"<b>[Progress]</b> Step: {progress.Current}, Status: {progress.Status}, Elapsed: <b>[{progress.Elapsed}]</b>, <small>Message: {progress.Message}</small>");
        }
        private async void TrainProxyWrapper_MulticlassMetricsUpdated(Microsoft.ML.Data.MulticlassClassificationMetrics metrics)
        {
            await _trainLogHub.Clients.All.SendAsync("Log", string.Format("Log-loss: {0}", metrics.LogLoss));
            await _trainLogHub.Clients.All.SendAsync("Log", string.Format("Log-loss measures the performance of a classifier with respect to how much the predicted probabilities diverge from the true class label. Lower log-loss indicates a better model. A perfect model, which predicts a probability of 1 for the true class, will have a log-loss of 0."));
            await _trainLogHub.Clients.All.SendAsync("Log", string.Format("Log-loss Reduction: {0}", metrics.LogLossReduction));
            await _trainLogHub.Clients.All.SendAsync("Log", string.Format("It gives a measure of how much a model improves on a model that gives random predictions. Log-loss reduction closer to 1 indicates a better model."));
            await _trainLogHub.Clients.All.SendAsync("Log", string.Format("Macro Accuracy: {0}", metrics.MacroAccuracy));
            await _trainLogHub.Clients.All.SendAsync("Log", string.Format("The accuracy for each class is computed and the macro-accuracy is the average of these accuracies. The macro-average metric gives the same weight to each class, no matter how many instances from that class the dataset contains."));
            await _trainLogHub.Clients.All.SendAsync("Log", string.Format("Micro Accuracy: {0}", metrics.MicroAccuracy));
            await _trainLogHub.Clients.All.SendAsync("Log", string.Format("The micro-average is the fraction of instances predicted correctly across all classes. Micro-average can be a more useful metric than macro-average if class imbalance is suspected."));
            await _trainLogHub.Clients.All.SendAsync("Log", string.Format("This is the relative number of examples where the true label one of the top K predicted labels by the predictor."));
            await _trainLogHub.Clients.All.SendAsync("Log", string.Format("Top K Prediction Count: {0}", metrics.TopKPredictionCount));
            await _trainLogHub.Clients.All.SendAsync("Log", string.Format("Top K Accuracy: {0}", metrics.TopKAccuracy));
            await _trainLogHub.Clients.All.SendAsync("Log", string.Format("If positive, this indicates the K in Top K Accuracy and Top K Accuracy for all K."));
            if (metrics.TopKAccuracyForAllK?.Count > 0)
            {
                await _trainLogHub.Clients.All.SendAsync("Log", string.Format("Top K Accuracy for all K: ({0})", string.Join(", ", metrics.TopKAccuracyForAllK)));
            }
            if (metrics.PerClassLogLoss?.Count > 0)
            {
                await _trainLogHub.Clients.All.SendAsync("Log", string.Format("Per Class Log-loss: ({0})", string.Join(", ", metrics.PerClassLogLoss)));
            }
        }
        #endregion
    }
}

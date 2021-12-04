using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ImageClassification.Core.Train;
using ImageClassification.Core.Train.Models;
using ImageClassification.Shared.Common;
using ImageClassification.Train.Common;
using Microsoft.ML;
using Microsoft.ML.Data;
using static Microsoft.ML.Vision.ImageClassificationTrainer;

namespace ImageClassification.Train
{
    public class Program
    {
#pragma warning disable IDE0060 // Remove unused parameter
        public static async Task Main(string[] args)
#pragma warning restore IDE0060 // Remove unused parameter
        {
            Console.WriteLine("Scenario `{0}` has been started", typeof(Program).Assembly.GetName().Name);

            #region Paths

            var currentDirectory = Directory.GetCurrentDirectory();
            var projectDirectory = Path.GetFullPath(Path.Combine(currentDirectory, "..", "..", ".."));
            var source = Path.Combine(projectDirectory, "assets", "inputs", "images", "data-set.zip");
            var destination = Path.Combine(projectDirectory, "assets", "outputs", "classifier.zip");

            #endregion

            DownloadDataSet(source);

            var trainer = new DefaultTrainWrapper(source)
            {
                MeasureTime = true
            };

            trainer.ImageMetricsUpdated += ConsoleImageMetricsUpdated;
            trainer.Log += ConsoleLog;
            trainer.MulticlassMetricsUpdated += ConsoleMulticlassMetricsUpdated;
            trainer.ProgressChanged += ConsoleProgressChanged;

            Directory.CreateDirectory(Path.GetDirectoryName(destination)!);
            if (File.Exists(destination))
            {
                File.Delete(destination);
            }

            var classifications = await trainer.TrainAsync(destination);

            #region Results

            Console.WriteLine();
            Console.WriteLine("Scenario `{0}` has been finished {1}",
                typeof(Program).Assembly.GetName().Name,
                (classifications.Count() > 0 ? "successfully" : "with failure, check the logs."));
            Console.WriteLine();

            #endregion
        }

        #region Handlers

        private static void ConsoleImageMetricsUpdated(ImageClassificationMetrics metrics)
        {
            Console.WriteLine(new string('*', 30));

            if (metrics.Train is { } trainMetrics)
            {
                Console.WriteLine("Accuracy: {0}", trainMetrics.Accuracy);
                Console.WriteLine("- Accuracy of the batch on this Epoch. Higher the better.");

                Console.WriteLine("Batch Processed Count: {0}", trainMetrics.BatchProcessedCount);
                Console.WriteLine("- The number of batches processed in an epoch.");

                Console.WriteLine("Cross-Entropy: {0}", trainMetrics.CrossEntropy);
                Console.WriteLine("- Cross-Entropy (loss) of the batch on this Epoch. Lower the better.");

                Console.WriteLine("Epoch: {0}", trainMetrics.Epoch);
                Console.WriteLine("- The training epoch index for which this metric is reported.");

                Console.WriteLine("Learning Rate: {0}", trainMetrics.LearningRate);
                Console.WriteLine("- Learning Rate used for this Epoch. Changes for learning rate scheduling.");
            }

            if (metrics.Bottleneck is { } bottleneckMetrics)
            {
                Console.WriteLine("Type of metrics: {0}", bottleneckMetrics.DatasetUsed);

                Console.WriteLine("Image with index {0} was processed out.", bottleneckMetrics.Index);
            }

            Console.WriteLine(new string('*', 30));
            Console.WriteLine();
        }

        private static void ConsoleLog(object sender, LoggingEventArgs e)
        {
            Console.WriteLine("~~~ {0}", e.Message);
        }

        private static void ConsoleMulticlassMetricsUpdated(MulticlassClassificationMetrics metrics)
        {
            Console.WriteLine(new string('=', 30));

            Console.WriteLine("Log-loss: {0}", metrics.LogLoss);
            Console.WriteLine(
                "Log-loss measures the performance of a classifier with respect to how much the predicted probabilities diverge from the true class label. Lower log-loss indicates a better model. A perfect model, which predicts a probability of 1 for the true class, will have a log-loss of 0.");
            Console.WriteLine("Log-loss Reduction: {0}", metrics.LogLossReduction);
            Console.WriteLine(
                "It gives a measure of how much a model improves on a model that gives random predictions. Log-loss reduction closer to 1 indicates a better model.");
            Console.WriteLine("Macro Accuracy: {0}", metrics.MacroAccuracy);
            Console.WriteLine(
                "The accuracy for each class is computed and the macro-accuracy is the average of these accuracies. The macro-average metric gives the same weight to each class, no matter how many instances from that class the dataset contains.");
            Console.WriteLine("Micro Accuracy: {0}", metrics.MicroAccuracy);
            Console.WriteLine(
                "The micro-average is the fraction of instances predicted correctly across all classes. Micro-average can be a more useful metric than macro-average if class imbalance is suspected.");
            Console.WriteLine(
                "This is the relative number of examples where the true label one of the top K predicted labels by the predictor.");
            Console.WriteLine("Top K Prediction Count: {0}", metrics.TopKPredictionCount);
            Console.WriteLine("Top K Accuracy: {0}", metrics.TopKAccuracy);
            Console.WriteLine("If positive, this indicates the K in Top K Accuracy and Top K Accuracy for all K.");
            if (metrics.TopKAccuracyForAllK?.Count > 0)
            {
                Console.WriteLine("Top K Accuracy for all K: ({0})", string.Join(", ", metrics.TopKAccuracyForAllK));
            }

            if (metrics.PerClassLogLoss?.Count > 0)
            {
                Console.WriteLine("Per Class Log-loss: ({0})", string.Join(", ", metrics.PerClassLogLoss));
            }

            Console.WriteLine();
            Console.WriteLine(metrics.ConfusionMatrix.GetFormattedConfusionTable());
            Console.WriteLine();

            Console.WriteLine(new string('=', 30));
        }

        private static void ConsoleProgressChanged(TrainProgress progress)
        {
            Console.WriteLine();
            Console.WriteLine(new string('-', 30));

            if (progress.Current is { } stepName)
            {
                Console.WriteLine("[{0}]", DateTime.Now);
                Console.WriteLine("Current step: {0}", stepName);
                Console.Write("Status: ");
                switch (progress.Status)
                {
                    case StepStatus.Started:
                        ConsoleHelper.ColorWriteLine(ConsoleColor.DarkYellow, progress.Status);
                        break;
                    case StepStatus.Finished:
                        ConsoleHelper.ColorWriteLine(ConsoleColor.DarkGreen, progress.Status);
                        break;
                    default:
                        ConsoleHelper.ColorWriteLine(ConsoleColor.Red, "Unknown");
                        break;
                }
            }

            Console.WriteLine("Message: {0}", progress.Message);

            if (progress.Elapsed is { } elapsed)
            {
                Console.WriteLine("Step took: {0}", elapsed);
            }

            Console.WriteLine(new string('-', 30));
            Console.WriteLine();
        }

        #endregion

        #region Helpers

        private static void DownloadDataSet(string fileName)
        {
            const string zip =
                @"https://github.com/bladehero/ImageClassification.DataSets/blob/master/data-set.zip?raw=true";
            const float percentPerSection = 0.02f;
            const int total = 1;
            var progress = new Progress<float>();
            Console.WriteLine($"Downloading archive:");
            Console.Write(new string('-', (int) (total / percentPerSection)));
            Console.SetCursorPosition(0, Console.CursorTop);

            var _lock = new object();
            progress.ProgressChanged += (_, percentage) =>
            {
                lock (_lock)
                {
                    if (Console.CursorLeft <= percentage / percentPerSection)
                    {
                        Console.Write('■');
                    }
                }
            };

            Web.Download(zip, Path.GetDirectoryName(fileName), Path.GetFileName(fileName), progress).Wait();
            Console.WriteLine();
            Console.WriteLine($"Downloaded");
        }

        #endregion
    }
}
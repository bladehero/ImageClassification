using ImageClassification.Core.Train.Attributes;
using ImageClassification.Core.Train.Interfaces;
using ImageClassification.Core.Train.Models;
using ImageClassification.Shared;
using Microsoft.ML;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace ImageClassification.Core.Train.Steps.Default
{
    [DefaultStepCapture]
    internal sealed class PrepareDataSetStep : BaseStep, ITrainStep<(string SourceDirectory, MLContext MLContext), IDataView>, ITrainStep
    {
        public override Stopwatch Stopwatch { get; set; }
        public override StepName StepName { get; } = StepName.PreparingDataSet;

        public override event Action<TrainProgress> Log;

        /// <summary>
        /// Image set using IDataView and shuffle for better balance
        /// </summary>
        /// <remarks>
        /// Second (2) step as default.
        /// </remarks>
        /// <param name="data">Path to directory where images will be stored and context of ML.NET for training model.</param>
        /// <returns>Shuffled full image file paths data set.</returns>
        public IDataView Execute((string SourceDirectory, MLContext MLContext) data)
        {
            (string source, MLContext mlContext) = data;

            if (string.IsNullOrWhiteSpace(source))
            {
                throw new ArgumentException("Path to archive cannot be null", nameof(source));
            }

            if (mlContext is null)
            {
                throw new NullReferenceException($"Parameter `{nameof(mlContext)}` was null!");
            }

            if (Directory.Exists(source) == false)
            {
                throw new DirectoryNotFoundException(source);
            }

            Log?.Invoke(GenerateStarted($"Preparing data set"));

            var images = FileUtils.LoadImagesFromDirectory(source, true)
                                  .Select(x => new ImageData(x.ImagePath, x.Label));
            var fullImagesDataset = mlContext.Data.LoadFromEnumerable(images);
            var shuffledFullImageFilePathsDataset = mlContext.Data.ShuffleRows(fullImagesDataset);

            Log?.Invoke(GenerateFinished($"Data set is prepared"));

            return shuffledFullImageFilePathsDataset;
        }

        protected override object ExecuteImpl(object data)
        {
            try
            {
                var castedData = ((string, MLContext))data;
                return Execute(castedData);
            }
            catch (InvalidCastException ex)
            {
                throw new ArgumentException("Argument has wrong format!", nameof(data), ex);
            }
        }
    }
}

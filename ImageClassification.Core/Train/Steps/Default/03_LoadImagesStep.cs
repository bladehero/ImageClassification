using ImageClassification.Core.Train.Attributes;
using ImageClassification.Core.Train.Interfaces;
using ImageClassification.Core.Train.Models;
using Microsoft.ML;
using System;
using System.Diagnostics;
using System.IO;
using static Microsoft.ML.Transforms.ValueToKeyMappingEstimator;

namespace ImageClassification.Core.Train.Steps.Default
{
    [DefaultStepCapture]
    internal sealed class LoadImagesStep : BaseStep, ITrainStep<(string SourceDirectory, MLContext MLContext, IDataView DataSet), IDataView>, ITrainStep
    {
        public override Stopwatch Stopwatch { get; set; }
        public override StepName StepName { get; } = StepName.LoadingImages;

        public override event Action<TrainProgress> Log;

        /// <summary>
        /// Load Images with in-memory type within the IDataView and Transform Labels to Keys (Categorical)
        /// </summary>
        /// <remarks>
        /// Third (3) step as default.
        /// </remarks>
        /// <param name="data">Path to directory where images will be stored, ML context and shuffled full image file paths data set.</param>
        /// <returns>Shuffled images in-memory data set.</returns>
        public IDataView Execute((string SourceDirectory, MLContext MLContext, IDataView DataSet) data)
        {
            (string source, MLContext mlContext, IDataView dataSet) = data;

            if (string.IsNullOrWhiteSpace(source))
            {
                throw new ArgumentException("Path to archive cannot be null", nameof(source));
            }

            if (mlContext is null)
            {
                throw new NullReferenceException($"Parameter `{nameof(mlContext)}` was null!");
            }

            if (dataSet is null)
            {
                throw new NullReferenceException($"Parameter `{nameof(dataSet)}` was null!");
            }

            if (Directory.Exists(source) == false)
            {
                throw new DirectoryNotFoundException(source);
            }

            Log?.Invoke(GenerateStarted($"Loading images in memory"));

            var shuffledFullImagesDataset =
                    mlContext.Transforms
                             .Conversion
                             .MapValueToKey("LabelAsKey",
                                            "Label",
                                            keyOrdinality: KeyOrdinality.ByValue)
                             .Append(mlContext.Transforms.LoadRawImageBytes("Image",
                                                                            source,
                                                                            "ImagePath"))
                             .Fit(dataSet)
                             .Transform(dataSet);

            Log?.Invoke(GenerateFinished($"Loading images in memory has been finished"));

            return shuffledFullImagesDataset;
        }

        protected override object ExecuteImpl(object data)
        {
            try
            {
                var castedData = ((string, MLContext, IDataView))data;
                return Execute(castedData);
            }
            catch (InvalidCastException ex)
            {
                throw new ArgumentException("Argument has wrong format!", nameof(data), ex);
            }
        }
    }
}

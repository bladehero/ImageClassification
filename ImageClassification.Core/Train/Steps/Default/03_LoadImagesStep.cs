using ImageClassification.Core.Train.Attributes;
using ImageClassification.Core.Train.Common;
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
                ThrowHelper.Argument("Path to archive cannot be null", nameof(source));
            }

            if (mlContext is null)
            {
                ThrowHelper.NullReference(nameof(mlContext));
            }

            if (dataSet is null)
            {
                ThrowHelper.NullReference(nameof(dataSet));
            }

            if (Directory.Exists(source) == false)
            {
                ThrowHelper.DirectoryNotFound(source);
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
                return ThrowHelper.Argument<object>("Argument has wrong format!", nameof(data), ex);
            }
        }
    }
}

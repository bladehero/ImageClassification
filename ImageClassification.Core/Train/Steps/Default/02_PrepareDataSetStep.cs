﻿using ImageClassification.Core.Train.Attributes;
using ImageClassification.Core.Train.Interfaces;
using ImageClassification.Core.Train.Models;
using ImageClassification.Shared;
using ImageClassification.Shared.Common;
using Microsoft.ML;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace ImageClassification.Core.Train.Steps.Default
{
    [DefaultStepCapture]
    internal sealed class PrepareDataSetStep : BaseStep, ITrainStep<(string SourceDirectory, MLContext MLContext), (IDataView DataSet, IEnumerable<string> Classifications)>, ITrainStep
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
        public (IDataView DataSet, IEnumerable<string> Classifications) Execute((string SourceDirectory, MLContext MLContext) data)
        {
            (string source, MLContext mlContext) = data;

            if (string.IsNullOrWhiteSpace(source))
            {
                ThrowHelper.Argument("Path to archive cannot be null", nameof(source));
            }

            if (mlContext is null)
            {
                ThrowHelper.NullReference(nameof(mlContext));
            }

            if (Directory.Exists(source) == false)
            {
                ThrowHelper.DirectoryNotFound(source);
            }

            Log?.Invoke(GenerateStarted($"Preparing data set"));

            var images = FileUtils.LoadImagesFromDirectory(source, true)
                                  .Select(x => new ImageData(x.ImagePath, x.Label));
            var fullImagesDataset = mlContext.Data.LoadFromEnumerable(images);
            var shuffledFullImageFilePathsDataset = mlContext.Data.ShuffleRows(fullImagesDataset);

            Log?.Invoke(GenerateFinished($"Data set is prepared"));

            return (shuffledFullImageFilePathsDataset, images.Select(x => x.Label));
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
                return ThrowHelper.Argument<object>("Argument has wrong format!", nameof(data), ex);
            }
        }
    }
}

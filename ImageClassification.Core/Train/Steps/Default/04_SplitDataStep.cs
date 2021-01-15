using ImageClassification.Core.Train.Attributes;
using ImageClassification.Core.Train.Common;
using ImageClassification.Core.Train.Interfaces;
using ImageClassification.Core.Train.Models;
using Microsoft.ML;
using System;
using System.Diagnostics;
using static Microsoft.ML.DataOperationsCatalog;

namespace ImageClassification.Core.Train.Steps.Default
{
    [DefaultStepCapture]
    internal sealed class SplitDataStep : BaseStep, ITrainStep<(MLContext MLContext, IDataView DataSet, double TestFraction), TrainTestData>, ITrainStep
    {
        public override Stopwatch Stopwatch { get; set; }
        public override StepName StepName { get; } = StepName.SplittingData;

        public override event Action<TrainProgress> Log;

        /// <summary>
        /// Split the data by formula as default `90:10` into train and test sets, train and evaluate.
        /// Change TestFraction to change the ratio.
        /// </summary>
        /// <remarks>
        /// Fourth (4) step as default.
        /// </remarks>
        /// <param name="data">ML context, shuffled images in-memory data set and test fraction.</param>
        /// <returns>Train and test data.</returns>
        public TrainTestData Execute((MLContext MLContext, IDataView DataSet, double TestFraction) data)
        {
            (MLContext mlContext, IDataView dataSet, double testFraction) = data;

            if (mlContext is null)
            {
                ThrowHelper.NullReference(nameof(mlContext));
            }

            if (dataSet is null)
            {
                ThrowHelper.NullReference(nameof(dataSet));
            }

            Log?.Invoke(GenerateStarted($"Data splitting is started"));
            var trainTestData = mlContext.Data.TrainTestSplit(dataSet, testFraction);
            Log?.Invoke(GenerateFinished($"Data has been splitted by formula Train/Test - {100 - testFraction * 100}/{testFraction * 100}"));

            return trainTestData;
        }

        protected override object ExecuteImpl(object data)
        {
            try
            {
                var castedData = ((MLContext, IDataView, double))data;
                return Execute(castedData);
            }
            catch (InvalidCastException ex)
            {
                return ThrowHelper.Argument<object>("Argument has wrong format!", nameof(data), ex);
            }
        }
    }
}

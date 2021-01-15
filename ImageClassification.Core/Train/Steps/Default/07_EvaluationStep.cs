using ImageClassification.Core.Train.Attributes;
using ImageClassification.Core.Train.Interfaces;
using ImageClassification.Core.Train.Models;
using Microsoft.ML;
using Microsoft.ML.Data;
using System;
using System.Diagnostics;

namespace ImageClassification.Core.Train.Steps.Default
{
    [DefaultStepCapture]
    internal sealed class EvaluationStep : BaseStep, ITrainStep<(MLContext MLContext, IDataView TestSet, ITransformer TrainedModel), MulticlassClassificationMetrics>, ITrainStep
    {
        public override Stopwatch Stopwatch { get; set; }
        public override StepName StepName { get; } = StepName.EvaluatingModel;

        public override event Action<TrainProgress> Log;

        /// <summary>
        /// Get the quality metrics (accuracy, etc.)
        /// </summary>
        /// <remarks>
        /// Seventh (7) step as default. Optional.
        /// </remarks>
        /// <param name="data">ML context, test data set and trained model.</param>
        /// <returns>Classification metrics.</returns>
        public MulticlassClassificationMetrics Execute((MLContext MLContext, IDataView TestSet, ITransformer TrainedModel) data)
        {
            (MLContext mlContext, IDataView testDataSet, ITransformer trainedModel) = data;

            if (mlContext is null)
            {
                throw new NullReferenceException($"Parameter `{nameof(mlContext)}` was null!");
            }

            if (testDataSet is null)
            {
                throw new NullReferenceException($"Parameter `{nameof(testDataSet)}` was null!");
            }

            if (trainedModel is null)
            {
                throw new NullReferenceException($"Parameter `{nameof(trainedModel)}` was null!");
            }

            Log?.Invoke(GenerateStarted($"Started evaluating model..."));

            var predictionsDataView = trainedModel.Transform(testDataSet);
            var metrics = mlContext.MulticlassClassification.Evaluate(predictionsDataView, labelColumnName: "LabelAsKey", predictedLabelColumnName: "PredictedLabel");

            Log?.Invoke(GenerateFinished($"Finished evaluating model"));

            return metrics;
        }

        protected override object ExecuteImpl(object data)
        {
            try
            {
                var castedData = ((MLContext, IDataView, ITransformer))data;
                return Execute(castedData);
            }
            catch (InvalidCastException ex)
            {
                throw new ArgumentException("Argument has wrong format!", nameof(data), ex);
            }
        }
    }
}

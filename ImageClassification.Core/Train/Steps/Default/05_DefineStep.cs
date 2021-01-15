using ImageClassification.Core.Train.Attributes;
using ImageClassification.Core.Train.Interfaces;
using ImageClassification.Core.Train.Models;
using Microsoft.ML;
using System;
using System.Diagnostics;
using static Microsoft.ML.Vision.ImageClassificationTrainer;

namespace ImageClassification.Core.Train.Steps.Default
{
    [DefaultStepCapture]
    internal sealed class DefineStep : BaseStep, ITrainStep<(MLContext MLContext, Options Options), IEstimator<ITransformer>>, ITrainStep
    {
        public override Stopwatch Stopwatch { get; set; }
        public override StepName StepName { get; } = StepName.DefiningModel;

        public override event Action<TrainProgress> Log;

        /// <summary>
        /// Define the model's training pipeline by using explicit hyper-parameters
        /// </summary>
        /// <remarks>
        /// Fifth (5) step as default.
        /// </remarks>
        /// <param name="data">ML context and image classification options.</param>
        /// <returns>Pipeline for trainning process.</returns>
        public IEstimator<ITransformer> Execute((MLContext MLContext, Options Options) data)
        {
            (MLContext mlContext, Options options) = data;

            if (mlContext is null)
            {
                throw new NullReferenceException($"Parameter `{nameof(mlContext)}` was null!");
            }

            if (options is null)
            {
                throw new NullReferenceException($"Parameter `{nameof(options)}` was null!");
            }

            Log?.Invoke(GenerateStarted($"Model will be defined with options:{Environment.NewLine}" +
                                        $"Epoch={options.Epoch}{Environment.NewLine}" +
                                        $"Architecture={options.Arch}{Environment.NewLine}" +
                                        $"BatchSize={options.BatchSize}{Environment.NewLine}" +
                                        $"LearningRate={options.LearningRate}{Environment.NewLine}" +
                                        $"FeatureColumnName={options.FeatureColumnName}{Environment.NewLine}" +
                                        $"LabelColumnName={options.FeatureColumnName}{Environment.NewLine}"));

            var pipeline =
                mlContext.MulticlassClassification
                         .Trainers
                         .ImageClassification(options)
                         .Append(mlContext.Transforms.Conversion.MapKeyToValue("PredictedLabel",
                                                                               "PredictedLabel"));

            Log?.Invoke(GenerateFinished($"Model is defined"));

            return pipeline;
        }

        protected override object ExecuteImpl(object data)
        {
            try
            {
                var castedData = ((MLContext, Options))data;
                return Execute(castedData);
            }
            catch (InvalidCastException ex)
            {
                throw new ArgumentException("Argument has wrong format!", nameof(data), ex);
            }
        }
    }
}

using ImageClassification.Core.Train.Attributes;
using ImageClassification.Core.Train.Interfaces;
using ImageClassification.Core.Train.Models;
using Microsoft.ML;
using System;
using System.Diagnostics;

namespace ImageClassification.Core.Train.Steps.Default
{
    [DefaultStepCapture]
    internal sealed class TrainStep : BaseStep, ITrainStep<(IEstimator<ITransformer> Pipeline, IDataView TrainSet), ITransformer>, ITrainStep
    {
        public override Stopwatch Stopwatch { get; set; }
        public override StepName StepName { get; } = StepName.Trainning;

        public override event Action<TrainProgress> Log;

        /// <summary>
        /// Train/create the ML model
        /// </summary>
        /// <remarks>
        /// Sixth (6) step as default.
        /// </remarks>
        /// <param name="data">Pipeline and train data set.</param>
        /// <returns>Trained model.</returns>
        public ITransformer Execute((IEstimator<ITransformer> Pipeline, IDataView TrainSet) data)
        {
            (IEstimator<ITransformer> pipeline, IDataView trainDataSet) = data;

            if (pipeline is null)
            {
                throw new NullReferenceException($"Parameter `{nameof(pipeline)}` was null!");
            }

            if (trainDataSet is null)
            {
                throw new NullReferenceException($"Parameter `{nameof(trainDataSet)}` was null!");
            }

            Log?.Invoke(GenerateStarted($"Started training process"));

            var trainedModel = pipeline.Fit(trainDataSet);

            Log?.Invoke(GenerateFinished($"Finished training process"));

            return trainedModel;
        }

        protected override object ExecuteImpl(object data)
        {
            try
            {
                var castedData = ((IEstimator<ITransformer>, IDataView))data;
                return Execute(castedData);
            }
            catch (InvalidCastException ex)
            {
                throw new ArgumentException("Argument has wrong format!", nameof(data), ex);
            }
        }
    }
}

using ImageClassification.Core.Train.Attributes;
using ImageClassification.Core.Train.Common;
using ImageClassification.Core.Train.Interfaces;
using ImageClassification.Core.Train.Models;
using Microsoft.ML;
using System;
using System.Diagnostics;
using System.IO;

namespace ImageClassification.Core.Train.Steps.Default
{
    [DefaultStepCapture]
    internal sealed class SaveStep : BaseStep, ITrainStep<(MLContext MLContext, ITransformer TrainedModel, DataViewSchema TrainSchema, Stream Stream), bool>, ITrainStep
    {
        public override Stopwatch Stopwatch { get; set; }
        public override StepName StepName { get; } = StepName.SavingModel;

        public override event Action<TrainProgress> Log;

        /// <summary>
        /// Save the model (ML.NET .zip model file and TensorFlow .pb model file)
        /// </summary>
        /// <remarks>
        /// Eighth (8) step as default.
        /// </remarks>
        /// <param name="data">Trained model, schema of train data set and a writeable, seekable stream to save to.</param>
        public bool Execute((MLContext MLContext, ITransformer TrainedModel, DataViewSchema TrainSchema, Stream Stream) data)
        {
            (MLContext mlContext, ITransformer trainedModel, DataViewSchema trainSchema, Stream stream) = data;

            if (mlContext is null)
            {
                ThrowHelper.NullReference(nameof(mlContext));
            }

            if (trainedModel is null)
            {
                ThrowHelper.NullReference(nameof(trainedModel));
            }

            if (trainSchema is null)
            {
                ThrowHelper.NullReference(nameof(trainSchema));
            }

            if (stream is null)
            {
                ThrowHelper.NullReference(nameof(stream));
            }

            if (!stream.CanRead)
            {
                ThrowHelper.InvalidOperation("Stream must be readable");
            }

            if (!stream.CanSeek)
            {
                ThrowHelper.InvalidOperation("Stream must be seakable");
            }

            Log?.Invoke(GenerateStarted($"Started saving model..."));

            mlContext.Model.Save(trainedModel, trainSchema, stream);

            Log?.Invoke(GenerateFinished($"Finished saving model"));

            return true;
        }

        protected override object ExecuteImpl(object data)
        {
            try
            {
                var castedData = ((MLContext, ITransformer, DataViewSchema, Stream))data;
                return Execute(castedData);
            }
            catch (InvalidCastException ex)
            {
                return ThrowHelper.Argument<object>("Argument has wrong format!", nameof(data), ex);
            }
        }
    }
}

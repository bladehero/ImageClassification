using ImageClassification.Core.Train.Interfaces;
using ImageClassification.Core.Train.Models;
using System;
using System.Diagnostics;

namespace ImageClassification.Core.Train.Steps
{
    public abstract class BaseStep : ITrainStep
    {
        public abstract Stopwatch Stopwatch { get; set; }
        public abstract StepName StepName { get; }
        public abstract event Action<TrainProgress> Log;

        public TrainProgress GenerateStarted(string message)
        {
            return new TrainProgress
            {
                Current = StepName,
                Message = message
            };
        }
        public TrainProgress GenerateFinished(string message)
        {
            return new TrainProgress
            {
                Current = StepName,
                Status = StepStatus.Finished,
                Message = message,
                Elapsed = Stopwatch?.Elapsed
            };
        }

        object ITrainStep.Execute(object data) => ExecuteImpl(data);
        protected abstract object ExecuteImpl(object data);
    }
}

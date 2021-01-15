using ImageClassification.Core.Train.Models;
using System;
using System.Diagnostics;

namespace ImageClassification.Core.Train.Interfaces
{
    internal interface ITrainStep
    {
        event Action<TrainProgress> Log;
        public Stopwatch Stopwatch { get; set; }
        StepName StepName { get; }
        object Execute(object data);
    }

    internal interface ITrainStep<D, out TResult> : ITrainStep
    {
        TResult Execute(D data);
    }
}

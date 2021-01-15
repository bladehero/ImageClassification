using ImageClassification.Core.Train.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace ImageClassification.Core.Train.Interfaces
{
    internal interface IStepCollection : IReadOnlyCollection<ITrainStep>, IEnumerable<ITrainStep>
    {
        Stopwatch Stopwatch { get; set; }

        event Action<TrainProgress> TrainLog;

        ITrainStep GetStep(StepName stepName);
        ITrainStep<D, TResult> GetStep<D, TResult>(StepName stepName);
    }
}
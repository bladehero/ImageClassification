using System;

namespace ImageClassification.Core.Train.DataModels
{
    public class TrainProgress
    {
        public TrainStepStatus? Current { get; set; }
        public string Message { get; set; }
        public TimeSpan? Elapsed { get; set; }
    }
}

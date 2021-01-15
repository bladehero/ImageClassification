using System;

namespace ImageClassification.Core.Train.Models
{
    public class TrainProgress
    {
        public StepName? Current { get; set; }
        public StepStatus Status { get; set; } = StepStatus.Started;
        public string Message { get; set; }
        public TimeSpan? Elapsed { get; set; }
    }
}

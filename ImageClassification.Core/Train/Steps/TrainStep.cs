namespace ImageClassification.Core.Train.Steps
{
    public interface TrainStep<D, out TResult>
    {
        TrainStepStatus Status { get; }
        TResult Execute(D data);
    }
}

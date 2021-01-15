namespace ImageClassification.Core.Train
{
    public enum TrainStepStatus
    {
        Initialization,
        Unarchiving,
        PreparingDataSet,
        LoadingImages,
        SplittingData,
        DefiningModel,
        Trainning,
        EvaluatingModel,
        SavingModel
    }
}

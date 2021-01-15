namespace ImageClassification.Core.Train.Models
{
    public enum StepName
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

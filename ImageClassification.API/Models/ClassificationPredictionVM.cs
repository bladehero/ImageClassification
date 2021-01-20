namespace ImageClassification.API.Models
{
    public class ClassificationPredictionVM
    {
        public string ImageId { get; set; }
        public string PredictedLabel { get; set; }
        public float Probability { get; set; }
        public long PredictionExecutionTime { get; set; }
    }
}

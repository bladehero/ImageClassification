namespace ImageClassification.API.Configurations
{
    public class MLModelOptions
    {
        public const string MLModel = "MLModel";
        public string MLModelFilePath { get; set; }
        public string WarmupImagePath { get; set; }
    }
}

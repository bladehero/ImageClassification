using ImageClassification.API.Interfaces;

namespace ImageClassification.API.Configurations
{
    public class MLModelOptions : IConfigurationOptions
    {
        public string SectionPath => "MLModel";

        public string MLModelFilePath { get; set; }
        public string WarmupImagePath { get; set; }
    }
}

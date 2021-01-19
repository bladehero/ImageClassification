using ImageClassification.API.Interfaces;

namespace ImageClassification.API.Configurations
{
    public class MLModelOptions : IConfigurationOptions
    {
        string IConfigurationOptions.SectionPath => "MLModel";

        public string MLModelFilePath { get; set; }
        public string WarmupImagePath { get; set; }
    }
}

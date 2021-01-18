using ImageClassification.API.Interfaces;

namespace ImageClassification.API.Configurations
{
    public class StorageOptions : IConfigurationOptions
    {
        public string SectionPath => "StorageOptions";

        public string StoragePath { get; set; }
    }
}

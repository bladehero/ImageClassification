using ImageClassification.API.Interfaces;

namespace ImageClassification.API.Configurations
{
    public class ImageSourceUploadOptions : IConfigurationOptions
    {
        string IConfigurationOptions.SectionPath => nameof(ImageSourceUploadOptions);

        public string StringBefore { get; set; }
        public string StringAfter { get; set; }
        public string LeftBracer { get; set; }
        public string RightBracer { get; set; }

        public string Build(object value) => $"{StringBefore}{LeftBracer}{value}{RightBracer}{StringAfter}";
    }
}

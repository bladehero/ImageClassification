using Newtonsoft.Json;

namespace ImageClassification.Preparation.Models.Unsplash
{
    public partial class Tag
    {
        [JsonProperty("title")]
        public string Title { get; set; }
    }
}

using Newtonsoft.Json;

namespace ImageClassification.Core.Preparation.Strategies.Unsplash.Internal
{
    internal class Tag
    {
        [JsonProperty("title")]
        public string Title { get; set; }
    }
}

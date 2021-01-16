using Newtonsoft.Json;
using System.Collections.Generic;

namespace ImageClassification.Core.Preparation.Strategies.Unsplash.Internal
{
    internal class Result
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("width")]
        public long Width { get; set; }

        [JsonProperty("height")]
        public long Height { get; set; }

        [JsonProperty("links")]
        public Link Links { get; set; }

        [JsonProperty("tags")]
        public List<Tag> Tags { get; set; }
    }
}

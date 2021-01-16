using System.Collections.Generic;
using Newtonsoft.Json;

namespace ImageClassification.Core.Preparation.Strategies.Unsplash.Internal
{
    internal class Response
    {
        [JsonProperty("total")]
        public int Total { get; set; }

        [JsonProperty("total_pages")]
        public int TotalPages { get; set; }

        [JsonProperty("results")]
        public IEnumerable<Result> Results { get; set; }
    }
}


using System.Collections.Generic;
using Newtonsoft.Json;

namespace ImageClassification.Preparation.Models.Unsplash
{
    public partial class Response
    {
        [JsonProperty("total")]
        public int Total { get; set; }

        [JsonProperty("total_pages")]
        public int TotalPages { get; set; }

        [JsonProperty("results")]
        public IEnumerable<Result> Results { get; set; }
    }
}


using Newtonsoft.Json;
using System;

namespace ImageClassification.Core.Preparation.Strategies.Unsplash.Internal
{
    internal class Link
    {
        [JsonProperty("download")]
        public Uri Download { get; set; }
    }
}

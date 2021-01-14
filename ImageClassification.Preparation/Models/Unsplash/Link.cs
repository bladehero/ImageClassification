using Newtonsoft.Json;
using System;

namespace ImageClassification.Preparation.Models.Unsplash
{
    public partial class Link
    {
        [JsonProperty("download")]
        public Uri Download { get; set; }
    }
}

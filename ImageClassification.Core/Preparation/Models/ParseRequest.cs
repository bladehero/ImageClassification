using System.Collections.Generic;

namespace ImageClassification.Core.Preparation.Models
{
    public class ParseRequest
    {
        /// <summary>
        /// Categories are used for parsing data.
        /// </summary>
        public IEnumerable<Category> Categories { get; set; }

        /// <summary>
        /// Estimated count per category
        /// </summary>
        public int EstimatedCount { get; set; }

        public ParseRequest()
        {
            Categories = new List<Category>();
        }
    }
}

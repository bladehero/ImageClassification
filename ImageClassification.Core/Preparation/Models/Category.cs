using System.Collections.Generic;

namespace ImageClassification.Core.Preparation.Models
{
    public class Category
    {
        public string Name { get; set; }
        public IEnumerable<string> Keywords { get; set; }

        public Category()
        {
            Keywords = new List<string>();
        }
    }
}

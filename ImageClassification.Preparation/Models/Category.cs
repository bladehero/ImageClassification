using System.Collections.Generic;

namespace ImageClassification.Preparation.Models
{
    public class Category
    {
        public string Name { get; set; }
        public List<string> Keywords { get; set; }

        public Category()
        {
            Keywords = new List<string>();
        }
    }
}

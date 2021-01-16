using System.Drawing;

namespace ImageClassification.Core.Preparation.Models
{
    public class ParsedImage
    {
        public string Category { get; set; }
        public string Keyword { get; set; }
        public Image Image { get; set; }
    }
}

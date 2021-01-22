namespace ImageClassification.API.Models
{
    public class StoredImageVM
    {
        public int Index { get; set; }
        public string Name { get; set; }
        public string Classification { get; set; }
        public string Folder { get; set; }
    }
}
